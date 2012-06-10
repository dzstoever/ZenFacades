using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Util;


//This class can be added to Windows Form applications
// to work with the Appender.Rtb of Zen.Log
namespace Zen.Log.Appenders
{
    /// <summary>
    /// Appends logging events to a RichTextBox
    /// </summary>
    /// <remarks>
    /// <para>
    /// RtbAppender appends log events to a specified RichTextBox control.
    /// It also allows the color, font and style of a specific type of message to be set.
    /// </para>
    /// <para>
    /// The RichTextBox property has to be set in code. The most straightforward way to accomplish
    /// this is in the Load event of the Form containing the control, or in the constructor
    /// <code lang="C#">
    /// private void MainForm_Load(object sender, EventArgs e)
    /// {
    ///    EIS.Log.Appenders.RtbAppender.SetRichTextBox(this.RtbLog, "Rtb");
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// When configuring the rich text box appender, mapping should be
    /// specified to map a logging level to a text style. For example:
    /// </para>
    /// <code lang="XML" escaped="true">
    ///  <mapping>
    ///    <level value="DEBUG" />
    ///    <textColorName value="DarkGreen" />
    ///  </mapping>
    ///  <mapping>
    ///    <level value="INFO" />
    ///    <textColorName value="ControlText" />
    ///  </mapping>
    ///  <mapping>
    ///    <level value="WARN" />
    ///    <textColorName value="Blue" />
    ///  </mapping>
    ///  <mapping>
    ///    <level value="ERROR" />
    ///    <textColorName value="Red" />
    ///    <bold value="true" />
    ///    <pointSize value="10" />
    ///  </mapping>
    ///  <mapping>
    ///    <level value="FATAL" />
    ///    <textColorName value="Black" />
    ///    <backColorName value="Red" />
    ///    <bold value="true" />
    ///    <pointSize value="12" />
    ///    <fontFamilyName value="Lucida Console" />
    ///  </mapping>  
    /// </code>
    /// <para>
    /// The Level is the standard log4net logging level. TextColorName and BackColorName should match 
    /// a value of the System.Drawing.KnownColor enumeration. Bold and/or Italic may be specified,using 
    /// <code>true</code> or <code>false</code>. FontFamilyName should match a font available on the client, 
    /// but if it's not found, the control's font will be used.
    /// </para>    
    /// </remarks>    
    public class RtbAppender : AppenderSkeleton
    {        
        #region Nested type: UpdateControlDelegate

        /// <summary>
        /// Delegate used to invoke UpdateControl
        /// </summary>
        /// <param name="loggingEvent">The event to log</param>
        /// <remarks>This delegate is used when UpdateControl must be 
        /// called from a thread other than the thread that created the 
        /// RichTextBox control.</remarks>
        private delegate void UpdateControlDelegate(LoggingEvent loggingEvent);

        #endregion

        #region Nested type: LevelTextStyle

        /// <summary>
        /// A class to act as a mapping between the level that a logging call is made at and
        /// the text style in which it should be displayed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Defines the mapping between a level and the text style in which it should be displayed..
        /// </para>
        /// </remarks>
        public class LevelTextStyle : LevelMappingEntry
        {
            private Color _backColor;
            private string _backColorName = "ControlLight";
            private bool _bold;
            private Font _font;
            private string _fontFamilyName;
            private FontStyle _fontStyle = FontStyle.Regular;
            private bool _italic;
            private float _pointSize;
            private Color _textColor;
            private string _textColorName = "ControlText";

            #region Properties

            /// <summary>
            /// Name of a KnownColor used as text background
            /// </summary>
            public string BackColorName
            {
                get { return _backColorName; }
                set { _backColorName = value; }
            }

            /// <summary>
            /// Display level in bold style
            /// </summary>
            public bool Bold
            {
                get { return _bold; }
                set { _bold = value; }
            }
            /// <summary>
            /// Name of a font family
            /// </summary>
            public string FontFamilyName
            {
                get { return _fontFamilyName; }
                set { _fontFamilyName = value; }
            }

            /// <summary>
            /// Display level in italic style
            /// </summary>
            public bool Italic
            {
                get { return _italic; }
                set { _italic = value; }
            }

            /// <summary>
            /// Font size of level, 0 to use default
            /// </summary>
            public float PointSize
            {
                get { return _pointSize; }
                set { _pointSize = value; }
            }
            /// <summary>
            /// Name of a KnownColor used for text
            /// </summary>
            public string TextColorName
            {
                get { return _textColorName; }
                set { _textColorName = value; }
            }

            internal Color BackColor
            {
                get { return _backColor; }
            }

            internal Font Font
            {
                get { return _font; }
            }
            internal FontStyle FontStyle
            {
                get { return _fontStyle; }
            }
            internal Color TextColor
            {
                get { return _textColor; }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initialize the options for the object
            /// </summary>
            /// <remarks>Parse the properties</remarks>
            public override void ActivateOptions()
            {
                base.ActivateOptions();
                _textColor = Color.FromName(_textColorName);
                _backColor = Color.FromName(_backColorName);
                if (_bold) _fontStyle |= FontStyle.Bold;
                if (_italic) _fontStyle |= FontStyle.Italic;

                if (_fontFamilyName != null)
                {
                    var size = _pointSize > 0.0f ? _pointSize : 8.25f;
                    try
                    {
                        _font = new Font(_fontFamilyName, size, _fontStyle);
                    }
                    catch (Exception)
                    {
                        _font = null;
                    }
                }
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        /// Reference to RichTextBox that displays logging events
        /// </summary>
        /// <remarks>
        /// <para> This property is a reference to the RichTextBox control that will display logging events.</para> 
        /// <para>If RichTextBox is null, no logging events will be displayed.</para>
        /// <para>RichTextBox will be set to null when the control's containing Form is closed.</para>
        /// </remarks>
        public RichTextBox RichTextBox
        {
            set
            {
                if (!ReferenceEquals(value, _richtextBox))
                {
                    if (_containerForm != null)
                    {
                        _containerForm.FormClosed -= ContainerForm_FormClosed;
                        _containerForm = null;
                    }

                    if (value != null)
                    {
                        value.ReadOnly = true;
                        value.HideSelection = false;

                        _containerForm = value.FindForm();
                        if (_containerForm != null) _containerForm.FormClosed += ContainerForm_FormClosed;
                    }

                    _richtextBox = value;
                }
            }
            get { return _richtextBox; }
        }
        private RichTextBox _richtextBox;

        /// <summary>
        /// Maximum number of characters in control before it is cleared
        /// </summary>
        public int MaxBufferLength
        {
            get { return _maxBufferLength; }
            set
            {
                if (value > 0)
                {
                    _maxBufferLength = value;
                }
            }
        }
        private int _maxBufferLength = 100000;
        
        /// <summary>
        /// This appender requires a "Layout" to be set.
        /// </summary>
        /// <value><c>true</c></value>
        /// <remarks>
        /// <para>
        /// This appender requires a "Layout" to be set.
        /// </para>
        /// </remarks>
        protected override bool RequiresLayout
        {
            get { return true; }
        }
        
        /// <summary>
        /// Mapping from level object to text style
        /// </summary>
        private readonly LevelMapping _levelMapping = new LevelMapping();
                
        /// <summary>
        /// Reference to Form that contains RichTextBox
        /// </summary>
        private Form _containerForm;
        
        #endregion

        #region Methods

        /// <summary>
        /// Overloaded method set the backcolor and font of the richTextBox, 
        /// before assigning it to the RtbAppender
        /// </summary>
        public static bool SetRichTextBox(RichTextBox richTextBox, string appenderName, Color backColor, Font font)
        {            
            richTextBox.Font = font;
            return SetRichTextBox(richTextBox, appenderName, backColor);
        }

        /// <summary>
        /// Overloaded method set the backcolor of the richTextBox, 
        /// before assigning it to the RtbAppender
        /// </summary>
        public static bool SetRichTextBox(RichTextBox richTextBox, string appenderName, Color backColor)
        {
            richTextBox.BackColor = backColor;
            return SetRichTextBox(richTextBox, appenderName); 
        }

        /// <summary>
        /// Assign a RichTextBox to a RtbAppender
        /// </summary>
        /// <param name="richTextBox">Reference to RichTextBox control that will display logging events</param>
        /// <param name="appenderName">Name of RtbAppender (case-sensitive)</param>
        /// <returns>True if a RtbAppender named <code>appenderName</code> was found</returns>
        /// <remarks>
        /// <para>This method sets the RichTextBox property of the RtbAppender
        /// in the default repository with <code>Name == appenderName</code>.</para>
        /// </remarks>
        /// <example>
        /// private void MainForm_Load(object sender, EventArgs e)
        /// {
        ///    log4net.Appender.RtbAppender.SetRichTextBox(logRichTextBox, "MainFormRichTextAppender");
        /// }        
        /// </example>
        public static bool SetRichTextBox(RichTextBox richTextBox, string appenderName)
        {
            if (appenderName == null) return false;

            var appenders = LogManager.GetRepository().GetAppenders();
            foreach (var appender in appenders.Where(appender => appender.Name == appenderName))
            {
                if (appender is RtbAppender)
                {
                    ((RtbAppender) appender).RichTextBox = richTextBox;
                    return true;
                }
                break;
            }
            return false;
        }

        /// <summary>
        /// Add a mapping of level to text style - done by the config file
        /// </summary>
        /// <param name="mapping">The mapping to add</param>
        /// <remarks>
        /// <para>
        /// Add a <see cref="LevelTextStyle"/> mapping to this appender.
        /// Each mapping defines the text style for a level.
        /// </para>
        /// </remarks>
        public void AddMapping(LevelTextStyle mapping)
        {
            _levelMapping.Add(mapping);
        }

        /// <summary>
        /// Initialize the options for this appender
        /// </summary>
        /// <remarks>
        /// <para>
        /// Initialize the level to text style mappings set on this appender.
        /// </para>
        /// </remarks>
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            _levelMapping.ActivateOptions();
        }

        /// <summary>
        /// This method is called by the <see cref="AppenderSkeleton.DoAppend(log4net.Core.LoggingEvent)"/> method.
        /// </summary>
        /// <param name="loggingEvent">The event to log.</param>
        /// <remarks>
        /// <para>
        /// Writes the event to the RichTextBox control, if set. 
        /// </para>
        /// <para>
        /// The format of the output will depend on the appender's layout.
        /// </para>
        /// <para>
        /// This method can be called from any thread.
        /// </para>
        /// </remarks>
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (_richtextBox != null)
            {
                if (_richtextBox.InvokeRequired)
                {
                    _richtextBox.Invoke(
                        new UpdateControlDelegate(UpdateControl),
                        new object[] {loggingEvent});
                }
                else
                {
                    UpdateControl(loggingEvent);
                }
            }
        }

        /// <summary>
        /// Remove references to container form
        /// </summary>
        protected override void OnClose()
        {
            base.OnClose();
            if (_containerForm != null)
            {
                _containerForm.FormClosed -= ContainerForm_FormClosed;
                _containerForm = null;
            }
        }
        
        /// <summary>
        /// Add logging event to configured control
        /// </summary>
        /// <param name="loggingEvent">The event to log</param>
        private void UpdateControl(LoggingEvent loggingEvent)
        {
            // There may be performance issues if the buffer gets too long
            // So periodically clear the buffer
            if (_richtextBox.TextLength > _maxBufferLength)
            {
                _richtextBox.Clear();
                _richtextBox.AppendText(
                    string.Format("(earlier messages cleared because log length exceeded maximum of {0})\n\n",
                                  _maxBufferLength));
            }

            // look for a style mapping
            var selectedStyle = _levelMapping.Lookup(loggingEvent.Level) as LevelTextStyle;
            if (selectedStyle != null)
            {
                // set the colors of the text about to be appended
                _richtextBox.SelectionBackColor = selectedStyle.BackColor;
                _richtextBox.SelectionColor = selectedStyle.TextColor;

                // alter selection font as much as necessary
                // missing settings are replaced by the font settings on the control
                if (selectedStyle.Font != null)
                {
                    // set Font Family, size and styles
                    _richtextBox.SelectionFont = selectedStyle.Font;
                }
                else if (selectedStyle.PointSize > 0 && _richtextBox.Font.SizeInPoints
                         != selectedStyle.PointSize)
                {
                    // use control's font family, set size and styles
                    var size = selectedStyle.PointSize > 0.0f
                                   ? selectedStyle.PointSize
                                   : _richtextBox.Font.SizeInPoints;
                    _richtextBox.SelectionFont = new Font(_richtextBox.Font.FontFamily.Name,
                                                          size, selectedStyle.FontStyle);
                }
                else if (_richtextBox.Font.Style != selectedStyle.FontStyle)
                {
                    // use control's font family and size, set styles
                    _richtextBox.SelectionFont = new Font(_richtextBox.Font, selectedStyle.FontStyle);
                }
            }

            _richtextBox.AppendText(RenderLoggingEvent(loggingEvent));
        }

        /// <summary>
        /// Remove reference to RichTextBox when container form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContainerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            RichTextBox = null;
        }

        #endregion
    }
}