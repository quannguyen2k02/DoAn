namespace Api1.Data
{
    public class OrderRequest
    {
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int TypePayment { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
