using System;

namespace Webshop.Models
{
  	public class Payload
    {
        public string[] products { get; set; }
        public Customer customer { get; set; }
        public Shipping shipping { get; set; }
        public PaymentInfo payment { get; set; }
    }

	public class Address
    {
        public string line1 { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country { get; set; }
    }

    public class Shipping
    {
        public string name { get; set; }
        public string phone { get; set; }
        public Address address { get; set; }
    }

    public class Customer
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public Address billing { get; set; }
    }

    public class PaymentInfo
    {
        public string cc_name { get; set; }
        public string cc_number { get; set; }
        public string cc_exp { get; set; }
        public string cc_csc { get; set; }
    }
}