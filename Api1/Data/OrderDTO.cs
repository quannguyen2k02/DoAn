namespace Api1.Data
{
    public class OrderDTO
    {

        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }


        public List<OrderDetailDTO> Items { get; set; }
    }
}
