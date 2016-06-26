using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Artefacts.Extensions
{
    public static class MemberInfoExtensions
    {
        public static object GetValue(this MemberInfo member, object instance)
        {
            PropertyInfo property = member as PropertyInfo;
            if (property != null)
                return property.GetValue(instance);
            FieldInfo field = member as FieldInfo;
            if (field != null)
                return field.GetValue(instance);
            throw new ArgumentOutOfRangeException(nameof(member), member, "Member should be a property or a field");
        }

		public static void SetValue(this MemberInfo member, object instance, object value)
		{
			PropertyInfo property = member as PropertyInfo;
			if (property != null)
				property.SetValue(instance, value);
			FieldInfo field = member as FieldInfo;
			if (field != null)
				field.SetValue(instance, value);
			throw new ArgumentOutOfRangeException(nameof(member), member, "Member should be a property or a field");
		}
	}
}
