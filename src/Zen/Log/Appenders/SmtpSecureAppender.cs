using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using log4net.Core;
using log4net.Appender;

/*
<!-- couldn't make this work without timeout issues for
  <appender name='Smtp'   type='EIS.Logging.Appenders.SmtpSecureAppender, EIS'>  
    <authentication value='Basic' />  
    <port value='465' />
    <username value='dan.stoever@gmail.com' />
    <password value='' />
-->
 */
namespace EIS.Log.Appenders
{
    /// <summary>Uses the SmtpClient to send SMTP mail that is secured via SSL.
    /// </summary>
    /// <remarks>The standard log4net SmtpAppender doesn't support SSL authentication</remarks>
    /// <see cref="http://mail-archives.apache.org/mod_mbox/logging-log4net-user/200602.mbox/%3C20060216123155.22007.qmail@web32202.mail.mud.yahoo.com%3E"/>
    public class SmtpSecureAppender : SmtpAppender
    {
        /// <summary>
        /// Use SmtpClient with .EnableSsl = true with UserCredentials
        /// </summary>
        override protected void SendBuffer(LoggingEvent[] events)
        {
            try
            {
                using (var writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture))
                {
                    if (Layout.Header != null) writer.Write(Layout.Header);
                    
                    foreach (var loggingEvent in events)
                        RenderLoggingEvent(writer, loggingEvent);// Render the event and append the text to the buffer
                    
                    if (Layout.Footer != null) writer.Write(Layout.Footer);
                    
                    var sslClient = new SmtpClient(SmtpHost, Port)
                    {// Use an SSL enabled SmtpClient
                        EnableSsl = true, 
                        Credentials = new NetworkCredential(Username, Password)
                    };
                    sslClient.Send(new MailMessage(From, To, Subject, writer.ToString()));
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Error occurred while sending e-mail notification from SmtpSecureAppender.", ex);
            }
        }
        
    } 
  
}