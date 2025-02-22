namespace Infrastructure.Services
{
    public class OrderStatusNameTranslateService
    {
        private readonly Dictionary<string, string> _statusNameTranslation;
        public OrderStatusNameTranslateService()
        {
            _statusNameTranslation = new Dictionary<string, string>
            {
                {"ConfirmedOrder", "Xác nhận" },
                {"PreparingOrder", "Chuẩn bị" },
                { "Pending", "Đang giao" },
                { "Done", "Đã giao" },
            };
        }

        public string Translate(string statusName)
        {
            return _statusNameTranslation.ContainsKey(statusName) ? _statusNameTranslation[statusName] : statusName;
        }

    }
}
