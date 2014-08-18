
/* user access (and security)
 **************************************************************************************************
 * Security is handled by a wcf service layer providing a single point of entry into the system
 * Access is granted based on token authentication (granted to an individual or group)
 * Users can ONLY interact with certain services or portions of services based on their token
 * All system services are defined by specific contracts
 *
 *	Note:	while we will refer to the 'user' in these use cases it is always the wcf service
 *			performing the actions and providing the results to one or more thin client(s).  
 *			This gives the system total control over a 'single point of entry' and the ability 
 *			to provide security and/or encryption for all critical, proprietery and sensitive 
 *			information presented to the end user (or other consumer of the service).
 *
 *	1 - The system can grant an authentication token to a user or group
 *	
 *	2 - The system can revoke an authentication token 
 *
 *	3 - The user is able to log into the system and be authenticated	
 *
 *	4 - The user is able to log out of the application
 *
 *	5 - The user can only access appropriate services and functions
 *		- the user is associated with the proper security level/context
 *		* all public service members should take ISecurityContext as a parameter
 *
 *	6 - The system provides encryption of sensitive information
 *
 */

namespace Zen.QZ.Manager.Tests.Common
{


    public class UserAccessScenarios
    {
         
    }
}