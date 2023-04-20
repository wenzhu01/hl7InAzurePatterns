using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;

[ServiceContract]
public interface IHl7Service
{
	[OperationContract]
	string ReceiveHL7Message(HL7Message hl7Message);	
}

[DataContract]
public class HL7Message
{
	[DataMember]
    public string Message { get; set; }
}
