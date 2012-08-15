--------------------------------------------------------------------------------------------------
Zen Overview
--------------------------------------------------------------------------------------------------
• Zen
	 .Core	- Domain model
	 .Data	- Data Access (IGenericDao - ISimpleDao) + Query model
	 .Ioc	- Inversion of Control/Dependancy Injection (IocDI) + default Impls
	 .Log	- Logging Interfaces (ILogger - ILoggerFactory) + default Impls
	 .Svcs	- WCF Service Contracts (Interfaces) + Data Contracts (Message Classes)
	 .Ux	- Application Controller (INavigationController) + View/ViewModel marker interfaces
	
• Zen.Boot
	- Application 'Shells' for loading run-time components required under different conditions 

• Zen.Data
	- NHibernateDao ORM Impl (default)

• Zen.Svcs
	- WCF Service Behaviors (Impl Classes) + ServiceHost/HostFactory (Impl Classes)

•*Zen.Svcs.Server
	- Stand-alone process to act as a server for Zen.Svcs.Hosts 

• Zen.Svcs.WebHost
	- WCF Web Host configuration for IIS containing the <system.serviceModel> for Zen.Svcs 

--------------------------------------------------------------------------------------------------
 Façades
--------------------------------------------------------------------------------------------------
•*Zen.Quartz.Server
	- a stand-alone process to act as a server for Quartz.Schedulers

•*Zen.Quartz.TopShelf
	- a 'shelved' component to act as a server for Quartz.Schedulers 
	  running under the Topshelf windows service 

 * Topshelf - allows a console app and windows to service share the same code. 	