public class Service : IHl7Service
{
    public string ReceiveHL7Message(HL7Message hl7Message)
    {
        // The logic here is not needed, this will be used as a passthru in APIM to modernize
        // So for now we will only return the message but technically here will be the validation etc 
        // We are moving this in the cloud
        string message = hl7Message.Message;
        return message;
    }
}
