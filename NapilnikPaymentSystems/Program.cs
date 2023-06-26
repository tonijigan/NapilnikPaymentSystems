using System;
using System.Security.Cryptography;
using System.Text;

namespace NapilnikPaymentSystems
{
    class Program
    {
        static void Main(string[] args)
        {
            const string urlLinkSber = "pay.system1.ru/order?amount=12000RUB&hash=";
            const string urlLinkVTB = "order.system2.ru/pay?hash=";
            const string urlLinkTinkoff = "system3.com/pay?amount=12000&curency=RUB&hash=";
            Order order = new Order(1300, 554);
            SberPay sberPay = new SberPay(urlLinkSber);
            VTBPay vTBPay = new VTBPay(urlLinkVTB);
            TinkoffPay tinkoffPay = new TinkoffPay(urlLinkTinkoff);
            Console.WriteLine(sberPay.GetPayingLink(order));
            Console.WriteLine(vTBPay.GetPayingLink(order));
            Console.WriteLine(tinkoffPay.GetPayingLink(order));
        }
    }

    public class Order
    {
        public readonly int Id;
        public readonly int Amount;

        public Order(int id, int amount) => (Id, Amount) = (id, amount);
    }

    public interface IPaymentSystem
    {
        string GetPayingLink(Order order);
    }
    abstract class BankingOperations
    {
        protected readonly string _urlLink;

        public BankingOperations(string urlLink)
        {
            _urlLink = urlLink;
        }

        public string GetHashMD5(int ID)
        {
            return Convert.ToBase64String(MD5.Create().
            ComputeHash(GetBytes(ID)));
        }

        public string GetHashSHA1(int amount)
        {
            return Convert.ToBase64String(SHA1.Create().
            ComputeHash(GetBytes(amount)));
        }

        private byte[] GetBytes(int value)
        {
            return Encoding.UTF8.GetBytes(value.ToString());
        }
    }

    class SberPay : BankingOperations, IPaymentSystem
    {
        public SberPay(string urlLink) : base(urlLink) { }

        public string GetPayingLink(Order order)
        {
            return $"{_urlLink}\nMD5 хеш ID - {GetHashMD5(order.Id)}\n";
        }
    }

    class VTBPay : BankingOperations, IPaymentSystem
    {
        public VTBPay(string urlLink) : base(urlLink) { }

        public string GetPayingLink(Order order)
        {
            return $"{_urlLink}\nMD5 хеш ID - {GetHashMD5(order.Id)}\n" +
                   $"Amount -{order.Amount}\n";
        }
    }

    class TinkoffPay : BankingOperations, IPaymentSystem
    {
        private string _secretKey = "w3th59gr2";

        public TinkoffPay(string urlLink) : base(urlLink) { }

        public string GetPayingLink(Order order)
        {
            return $"{_urlLink}\nSHA1 - {GetHashSHA1(order.Amount)}\n" +
                   $"ID - {order.Id}\nСекретный ключ - {_secretKey}\n";
        }
    }
}