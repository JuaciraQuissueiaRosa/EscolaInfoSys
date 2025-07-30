using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EscolaInfoSys.Helper
{
    public static class EnumHelper
    {
        public static IEnumerable<SelectListItem> GetSelectList<T>() where T : Enum
        {
            var type = typeof(T);
            var values = Enum.GetValues(type).Cast<T>();

            foreach (var val in values)
            {
                var memInfo = type.GetMember(val.ToString());
                var displayAttr = memInfo[0].GetCustomAttribute<DisplayAttribute>();
                var name = displayAttr != null ? displayAttr.Name : val.ToString();

                yield return new SelectListItem
                {
                    Value = Convert.ToInt32(val).ToString(),
                    Text = name
                };
            }
        }
    }
}
