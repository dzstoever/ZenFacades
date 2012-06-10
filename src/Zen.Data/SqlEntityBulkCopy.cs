using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Metadata;

namespace Zen.Data
{
    public class SqlEntityBulkCopy
    {
        internal static Configuration Cfg; 
        internal static ISessionFactoryImplementor SessionFactoryImpl;
                
        
        /// <summary>
        /// Wrapper class to produce an Ado.Net Datatable from any entity, 
        /// and perform SqlBulkCopy operations
        /// </summary>
        public SqlEntityBulkCopy(string sqlCnnString, Type entityType)
        {

            if (Cfg == null)
            {
                //Note: The NHibernate.Cfg.Configuration is meant only as an initialization-time object. 
                //Note: NHibernate.ISessionFactory is immutable and does not retain any association back to the Session

                Cfg = new Configuration();
                //Cfg.SetProperty("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
                Cfg.SetProperty("dialect", "NHibernate.Dialect.MsSql2008Dialect");
                Cfg.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
                Cfg.SetProperty("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
                Cfg.SetProperty("connection.connection_string", sqlCnnString);

                //add all the mappings embedded in this assembly
                Cfg.AddAssembly(typeof(SqlEntityBulkCopy).Assembly);

                var sessionFactory = Cfg.BuildSessionFactory();
                SessionFactoryImpl = (ISessionFactoryImplementor)sessionFactory;
            }
            EntityType = entityType;
            //_session = SessionFactoryImpl.OpenSession();
            _metaData = SessionFactoryImpl.GetClassMetadata(EntityType);            
            _persistentClass = Cfg.GetClassMapping(EntityType);            
            _sqlCnn = new SqlConnection(sqlCnnString);
            _sqlBulkCopy = new SqlBulkCopy(_sqlCnn);
            
            //Debug.WriteLine("EntityName = " + _metaData.EntityName);
            //Debug.WriteLine("IdentifierPropertyName = " + _metaData.IdentifierPropertyName);
            //Debug.WriteLine("IdentifierType = " + _metaData.IdentifierType);

            BuildDataTable();
            BuildAndMapSqlBulkCopy();
        }

        
        public SqlBulkCopy SqlBulkCopy
        {
            get
            {
                if (_sqlBulkCopy == null) BuildAndMapSqlBulkCopy();
                return _sqlBulkCopy;
            }
        }

        public DataTable BulkTable { get; private set; }

        public DataTable DataTable { get; private set; }

        public Type EntityType { get; private set; }
        
        
        private readonly SqlBulkCopy _sqlBulkCopy;
        private readonly SqlConnection _sqlCnn;
        private readonly PersistentClass _persistentClass;
        private readonly IClassMetadata _metaData;
        

        public string GetSqlTableName()
        {   
            return _persistentClass.Table.Name;            
        }

        public string GetSqlColumnName(string propertyName)
        {
            foreach (Column dbCol in from prop in _persistentClass.PropertyIterator 
                                     where prop.Name == propertyName 
                                     from Column dbCol in prop.ColumnIterator 
                                     select dbCol)
            {
                return dbCol.Name;
            }
            throw new Exception("Property not found! (" + propertyName + ")");
        }

        public void AddDataRow(object entity)
        {
            if (entity.GetType() != EntityType)
            { throw new Exception("Invalid Entity Type!"); }

            AddRow(DataTable, entity);
        }

        public void AddBulkCopyRow(object entity)
        {
            if (entity.GetType() != EntityType)
            { throw new Exception("Invalid Entity Type!"); }

            AddRow(BulkTable, entity);

        }

        public void ExecuteBulkCopy()
        {
            _sqlCnn.Open();
            try
            {
                _sqlBulkCopy.WriteToServer(BulkTable);
            }
            finally
            {
                BulkTable.Clear();
                _sqlCnn.Close();
            }
            //_sqlConnection.Close();
        }


        private void BuildDataTable()
        {
            DataTable = new DataTable(EntityType.Name);
            foreach (var propName in _metaData.PropertyNames)
            {
                var nhType = _metaData.GetPropertyType(propName);
                if (!nhType.IsAssociationType && !nhType.IsCollectionType)
                    DataTable.Columns.Add(new DataColumn(propName, nhType.ReturnedClass));
            }
        }

        private void BuildAndMapSqlBulkCopy()
        {

            BulkTable = new DataTable("BULK_" + EntityType.Name);
            SqlBulkCopy.DestinationTableName = _persistentClass.Table.Name;//Database table name                        

            foreach (var prop in _persistentClass.PropertyIterator)
            {
                if (!prop.IsBasicPropertyAccessor || !prop.IsInsertable || prop.Type.IsCollectionType) continue;
                var colName = prop.Name;
                foreach (Column dbCol in prop.ColumnIterator)
                {
                    var colType = dbCol.Value.Type.ReturnedClass;
                    if (prop.Type.IsAssociationType)
                    {
                        //use the pk from associated class for the columnType
                        var assClass = Cfg.GetClassMapping(colType);
                        foreach (Column assCol in assClass.Identifier.ColumnIterator)
                            colType = assCol.Value.Type.ReturnedClass;                            
                    }

                    if (prop.Type.IsComponentType)
                        colName = string.Format("{0}~{1}", prop.Name, dbCol.Name);                        

                    //Logger.DebugFormat("ColumnName: {0}, Type: {1}", colName, colType);
                    BulkTable.Columns.Add(new DataColumn(colName, colType));
                    SqlBulkCopy.ColumnMappings.Add(colName, dbCol.Name);
                }
            }

        }

        private void AddRow(DataTable table, object entity)
        {
            var row = table.NewRow();
            var props = EntityType.GetProperties();
            foreach (var prop in props)
            {
                if (!table.Columns.Contains(prop.Name)) continue;
                var val = prop.GetValue(entity, null);
                row[prop.Name] = val ?? DBNull.Value;
                //else { Debug.WriteLine("Excluded value = " + colName); }
            }
            table.Rows.Add(row);
        }
    }
}