
SOA Design Practices
--------------------------------------------------------------------------------------------------

Service Contracts (Interfaces)
------------------------------
• intentionally kept simple and concise 

• housed in the framework assembly (seperate from the Behaviors) since they should not change and
	we want to allow behaviors to be modified and enhanced frequently but not affect the client- 
	side consumers who should only be dependent on clean and well-defined service contracts. 
	In other words we don't want the clients to be affected every time we make a change to the 
	underlying behavior of a given service. *We could potentially put the contracts in their own
	dedicated assembly if we don't want consumers to be dependent on the Zen Framework.


Service Behaviors (Implementation Classes)
------------------------------------------
• housed in a dedicated assembly for the same reasons discussed above. This allows development 
	to continue on the services and maintenance to be applied at the host level while giving us 
	the ability to change and enhance backend functionality in an agile manner. We can release 
	new versions of the Zen.Svcs assembly with completely new or updated service implementations 
	to leverage new technologies or a different backend models as long as we still support the 
	same contract interfaces for the clients. The clients will be aware of any updates through 
	the BaseResponse.Version property. If a client requires a minimum version of Zen.Svcs the host-
	side will know whether it can support the client through the BaseRequest.Version property. 

• for the sake of SCALABILITY, all Services should have their InstanceContextMode = PerCall, and
	thus be developed in a 'stateless' manner. This allows for maximumum flexibility and is a
	general best practice so that the services can scale beyond one machine and avoid concurrency 
	nightmares. This is especially important for use with the DynamicRouter. When it is necessary 	
	to store state information for the service between calls, like access tokens for instance, 
	the data should be stored in a database along with any necessary caching logic to allow for 
	maximum performance. 


Hosting
-------
• yada


Security
--------
• yada
