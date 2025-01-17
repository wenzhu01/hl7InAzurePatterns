﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoapClient.HL7Srv {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="HL7Message", Namespace="http://schemas.datacontract.org/2004/07/")]
    [System.SerializableAttribute()]
    public partial class HL7Message : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="HL7Srv.IHl7Service")]
    public interface IHl7Service {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHl7Service/ReceiveHL7Message", ReplyAction="http://tempuri.org/IHl7Service/ReceiveHL7MessageResponse")]
        string ReceiveHL7Message(SoapClient.HL7Srv.HL7Message hl7Message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHl7Service/ReceiveHL7Message", ReplyAction="http://tempuri.org/IHl7Service/ReceiveHL7MessageResponse")]
        System.Threading.Tasks.Task<string> ReceiveHL7MessageAsync(SoapClient.HL7Srv.HL7Message hl7Message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IHl7ServiceChannel : SoapClient.HL7Srv.IHl7Service, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Hl7ServiceClient : System.ServiceModel.ClientBase<SoapClient.HL7Srv.IHl7Service>, SoapClient.HL7Srv.IHl7Service {
        
        public Hl7ServiceClient() {
        }
        
        public Hl7ServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Hl7ServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Hl7ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Hl7ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string ReceiveHL7Message(SoapClient.HL7Srv.HL7Message hl7Message) {
            return base.Channel.ReceiveHL7Message(hl7Message);
        }
        
        public System.Threading.Tasks.Task<string> ReceiveHL7MessageAsync(SoapClient.HL7Srv.HL7Message hl7Message) {
            return base.Channel.ReceiveHL7MessageAsync(hl7Message);
        }
    }
}
