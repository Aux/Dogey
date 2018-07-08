// https://github.com/Microsoft/referencesource/blob/master/System.ComponentModel.DataAnnotations/DataAnnotations/RangeAttribute.cs

using Discord.Commands;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;

namespace Dogey
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class RangeAttribute : ParameterPreconditionAttribute
    {
        public object Minimum { get; private set; }
        public object Maximum { get; private set; }
        public Type OperandType { get; private set; }

        private Func<object, object> Conversion { get; set; }

        public RangeAttribute(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
            OperandType = typeof(int);
        }
        public RangeAttribute(double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
            OperandType = typeof(double);
        }
        public RangeAttribute(Type type, string minimum, string maximum)
        {
            OperandType = type;
            Minimum = minimum;
            Maximum = maximum;
        }
        
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            if (IsValid(value))
                return Task.FromResult(PreconditionResult.FromSuccess());
            return Task.FromResult(PreconditionResult.FromError($"'{parameter.Name}' must be greater than `{Minimum}` and less than `{Maximum}`"));
        }

        private void Initialize(IComparable minimum, IComparable maximum, Func<object, object> conversion)
        {
            if (minimum.CompareTo(maximum) > 0)
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "{0} must be greater than {1}", maximum, minimum));
            
            Minimum = minimum;
            Maximum = maximum;
            Conversion = conversion;
        }

        public bool IsValid(object value)
        {
            SetupConversion();
            
            if (value == null)
                return true;
            
            string s = value as string;
            if (s != null && String.IsNullOrEmpty(s))
                return true;
            
            object convertedValue = null;

            try
            {
                convertedValue = this.Conversion(value);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

            IComparable min = (IComparable)Minimum;
            IComparable max = (IComparable)Maximum;
            return min.CompareTo(convertedValue) <= 0 && max.CompareTo(convertedValue) >= 0;
        }

        private void SetupConversion()
        {
            if (Conversion == null)
            {
                object minimum = Minimum;
                object maximum = Maximum;

                if (minimum == null || maximum == null)
                    throw new InvalidOperationException("Must set min and max");
                
                Type operandType = minimum.GetType();

                if (operandType == typeof(int))
                    Initialize((int)minimum, (int)maximum, v => Convert.ToInt32(v, CultureInfo.InvariantCulture));
                else if (operandType == typeof(double))
                    Initialize((double)minimum, (double)maximum, v => Convert.ToDouble(v, CultureInfo.InvariantCulture));
                else
                {
                    Type type = this.OperandType;
                    if (type == null)
                        throw new InvalidOperationException("Must set operand type");
                    
                    Type comparableType = typeof(IComparable);
                    if (!comparableType.IsAssignableFrom(type))
                        throw new InvalidOperationException(String.Format( CultureInfo.CurrentCulture, "Arbitrary types `{0}` and `{1}` not IComparable", type.FullName, comparableType.FullName));
                    
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    IComparable min = (IComparable)converter.ConvertFromString((string)minimum);
                    IComparable max = (IComparable)converter.ConvertFromString((string)maximum);

                    Func<object, object> conversion = value => (value != null && value.GetType() == type) ? value : converter.ConvertFrom(value);

                    Initialize(min, max, conversion);
                }
            }
        }
    }
}
