using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System;

/// <summary>
/// 当前时间指定类型   
/// （注：Linq查询时不可直接调用 DateTime.Now，否则无结果！！！，必须调用该结构属性或DateTimeExtensions扩展方法SetKind...()的结果）
/// </summary>
public static class DateTimeNow
{
    /// <summary>
    /// 系统时间
    /// </summary>
    public static DateTime Local { get; set; } = DateTime.Now.SetKindLocal();
    /// <summary>
    /// 协调世界时(UTC)
    /// </summary>
    public static DateTime Utc { get; set; } = DateTime.Now.SetKindUtc();
    /// <summary>
    /// 未指定类型
    /// </summary>
    public static DateTime Unspecified { get; set; } = DateTime.Now.SetKindUnspecified();
}

/// <summary>
/// DateTime扩展
/// （注：Linq查询时不可直接调用 DateTime.Now，否则无结果！！！，必须调用该扩展方法SetKind...()或DateTimeNow结构属性的结果）
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// 转换为 (UTC)协调世界时
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime? SetKindUtc(this DateTime? dateTime)
    {
        if (dateTime.HasValue)
        {
            return dateTime.Value.SetKindUtc();
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 转换为 (UTC)协调世界时
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime SetKindUtc(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc) { return dateTime; }
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    /// <summary>
    /// 转换为 本地时间
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime? SetKindLocal(this DateTime? dateTime)
    {
        if (dateTime.HasValue)
        {
            return dateTime.Value.SetKindLocal();
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 转换为 本地时间
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime SetKindLocal(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Local) { return dateTime; }
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
    }

    /// <summary>
    /// 转换Unspecified类型（无指定）
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime? SetKindUnspecified(this DateTime? dateTime)
    {
        if (dateTime.HasValue)
        {
            return dateTime.Value.SetKindLocal();
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 转换Unspecified类型（无指定）
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime SetKindUnspecified(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified) { return dateTime; }
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
    }


}





