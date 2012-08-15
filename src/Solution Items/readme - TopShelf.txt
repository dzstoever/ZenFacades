
Topshelf.Host
---------------------------------------------------------------------------------------------------
A wcf service host that can be run as a console application, or as a windows service.
It will look in the directory it is installed in, for the directory called 'Services'. 
Every folder in 'Services' gets its implementation launched as a service under the service host.

Note:	implementation class must inherit Topshelf.ShelvingBootstrapper and provide the
		.InitializeHostedService(IServiceConfigurator<T) method


Running the Host
---------------------------------------------------------------------------------------------------
1) To run the host as a console app just start the executable.
2) Run `Topshelf.Host install` in order to install the host as a windows service.

Note: 	If you want to change the format or destination for messages 
		you can add edit the log4net.config file.

		
Host Description
---------------------------------------------------------------------------------------------------
The host will start two wcf services by default:
- [TopshelfDashboard] Uri=> http://localhost:8483/Topshelf/Topshelf.Host 
	- this lists the hosted services, but the actions(Play/Stop) don't seem to work...

- [TopShelf.DirectoryMonitor] 
	@ net.pipe://localhost/topshelf_2420/controller/TopShelf.DirectoryMonitor(2420/TopShelf.DirectoryMonitor) 	
	- this will monitor the 'Services' folder for new 'Shelves'	
	- for each 'Shelf' it will create a host channel.(SHELFNAME = folder name under 'Services')
	@ net.pipe://localhost/topshelf_2420/controller/SHELFNAME(2420/SHELFNAME) 
	
	
Shelving Description
---------------------------------------------------------------------------------------------------
- Shelved services are just DLL assemblies, logical services will run the Topshelf.Host program

- When you drop files in a directory, it looks for the interface implementation and spins up a 
  new AppDomain with that logical service isolated in that AppDomain. If you update the files in 
  that folder, the service will attempt to cleanly shutdown then restart with the new files. 
		

Configuration
---------------------------------------------------------------------------------------------------	
- Each logical service can provide it's own configuration by including a SHELFNAME.config file
- Each logical service is responsible for providing it's own logging.

<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="ShelfConfiguration" type="Topshelf.Shelving.ShelfConfiguration, TopShelf" />
  </configSections>

  <ShelfConfiguration Bootstrapper="MyShelfAssembly.MyBootStrapperImplClass, MyShelfAssembly" />
  
  <appSettings>
    <add key="someName" value="someValue" />
  </appSettings>
  
</configuration>

	
Further Details: 
http://topshelf-project.com/
http://topshelf-project.com/documentation/shelving/
http://www.christophdebaene.com/blog/2011/03/16/create-a-net-windows-service-in-5-steps-with-topshelf/