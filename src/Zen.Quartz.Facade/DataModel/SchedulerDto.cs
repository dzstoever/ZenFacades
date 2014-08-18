using System;

namespace Zen.Quartz.Facade.DataModel
{
    public class SchedulerDto
    {
        public Guid Id { get; set; }//include Id from the base class in the Dto ?
        //Note: Uid, Version, and CreateDate are not in the Dto, but could be...? 

        public string Name { get; set; }
        public string Cluster { get; set; }
        public string SchedType { get; set; }
    }
}