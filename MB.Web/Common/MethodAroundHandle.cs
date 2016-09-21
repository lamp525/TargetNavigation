using System.Text;
using MB.New.Common;

namespace MB.Web.Common
{
    public class MethodAroundHandle : AopAlliance.Intercept.IMethodInterceptor
    {
        public object Invoke(AopAlliance.Intercept.IMethodInvocation invocation)
        {
            LogHelper.Output("DebugInformation", EnumDefine.ErrorLevel.DEBUG, string.Format("Call Method Start: '{0}'", invocation.Method.Name));

            StringBuilder message = new StringBuilder();
            message.Append("Input Parameters: ");

            if (invocation.Arguments != null)
            {
                foreach (object arg in invocation.Arguments)
                {
                    message.Append(arg.ToString());
                    message.Append(" || ");
                }
            }

            LogHelper.Output("DebugInformation", EnumDefine.ErrorLevel.DEBUG, message.ToString());

            object returnValue = invocation.Proceed();

            LogHelper.Output("DebugInformation", EnumDefine.ErrorLevel.DEBUG, string.Format("Call Method End: '{0}'", invocation.Method.Name));

            return returnValue;
        }
    }
}