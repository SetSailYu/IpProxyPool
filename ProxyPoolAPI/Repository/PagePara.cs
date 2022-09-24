namespace ProxyPoolAPI.Repository
{
    /// <summary>
    /// 分页参数类
    /// </summary>
    public class PagePara
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
    public class PageData<T> : PagePara
    {
        public int Count { get; set; }
        public int PageCount { get => Convert.ToInt32(Math.Ceiling(Count / (double)PageSize)); }
        public List<T> ListData { get; set; }
    }
}
