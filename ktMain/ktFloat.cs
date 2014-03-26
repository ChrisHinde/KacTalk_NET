using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KacTalk;
using System.Globalization;

namespace ktMainLib
{
    public class ktFloat : ktClass
    {
        public ktFloat(float value)
            : base("ktFloat")
        {
            m_value = value;
        }
        public ktFloat() : this(0.0f) { }

        public ktFloat(ktFloat val)
            : base("ktFloat")
        {
            m_value = val.m_value;
            m_HardType = val.m_HardType;
            m_IsClass = val.m_IsClass;
            m_Parent = val.m_Parent;
            m_IsConstant = val.m_IsConstant;
        }

        /// <summary>
        /// Run a method
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        public override ktValue _RunMethod(ktString Name, ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            // Check the name of the method to call and call appropriate method
            switch (Name.AsLower())
            {
                case "_add":
                case "op+":
                case "operator+":
                    {
                        Value = _Add(Arguments);
                        break;
                    }
                case "add":
                case "op+=":
                case "operator+=":
                case "_append":
                    {
                        CheckIfConstant(Name);
                        Value = Add(Arguments);
                        break;
                    }
                case "increase":
                case "op++":
                case "operator++":
                case "_increase":
                    {
                        CheckIfConstant(Name);
                        Value = Increase();
                        break;
                    }
                case "_subtract":
                case "op-":
                case "operator-":
                    {
                        Value = _Subtract(Arguments);
                        break;
                    }
                case "op-=":
                case "operator-=":
                case "subtract":
                    {
                        CheckIfConstant(Name);
                        Value = Subtract(Arguments);
                        break;
                    }
                case "decrease":
                case "op--":
                case "operator--":
                case "_decrease":
                    {
                        CheckIfConstant(Name);
                        Value = Decrease();
                        break;
                    }
                case "_multiply":
                case "_times":
                case "op*":
                case "operator*":
                    {
                        Value = _Multiply(Arguments);
                        break;
                    }
                case "multiply":
                case "times":
                case "op*=":
                case "operator*=":
                    {
                        CheckIfConstant(Name);
                        Value = Multiply(Arguments);
                        break;
                    }
                case "_divide":
                case "op/":
                case "operator/":
                    {
                        Value = _Divide(Arguments);
                        break;
                    }
                case "op/=":
                case "operator/=":
                case "divide":
                    {
                        CheckIfConstant(Name);
                        Value = Divide(Arguments);
                        break;
                    }
                case "_mod":
                case "_modulus":
                case "op%":
                case "operator%":
                    {
                        Value = _Modulus(Arguments);
                        break;
                    }
                case "op%=":
                case "operator%=":
                case "modulus":
                    {
                        CheckIfConstant(Name);
                        Value = Modulus(Arguments);
                        break;
                    }
                case "_power":
                case "_pow":
                case "op**":
                case "operator**":
                    {
                        Value = _Power(Arguments);
                        break;
                    }
                case "op^":
                case "operator^":
                    {
                        if (kacTalk.Main.MathMode)
                        {
                            Value = _Power(Arguments);
                        }
                        else
                        {
                         //   Value = _ExclusiveOr(Arguments);
                            return Value;
                        }
                        break;
                    }
                case "op^=":
                case "operator^=":
                    {
                        CheckIfConstant(Name);
                        if (kacTalk.Main.MathMode)
                        {
                            Value = Power(Arguments);
                        }
                        else
                        {
                            //Value = ExclusiveOr(Arguments);
                            return Value;
                        }
                        break;
                    }
                case "power":
                case "pow":
                    {
                        CheckIfConstant(Name);
                        Value = Power(Arguments);
                        break;
                    }
                case "_assign":
                case "op=":
                case "operator=":
                case "assign":
                    {
                        CheckIfConstant(Name);
                        Value = Assign(Arguments);
                        break;
                    }
                case "toint":
                    {
                        double d = Math.Round(m_value);
                        Value = new ktValue("return", "ktInt", new ktInt((int)d), true, true);
                        break;
                    }
                case "round":
                    {
                        Value = Round(Arguments);
                        break;
                    }
                /*case "tobase":
                    {
                        Value = ToBase(Arguments);
                        break;
                    }
                case "tobinary":
                    {
                        Value = _ToBase(2);
                        break;
                    }
                case "tobool":
                    {
                        Value = _ToBool();
                        break;
                    }
                case "tooct":
                    {
                        Value = _ToBase(8);
                        break;
                    }
                case "tohex":
                    {
                        Value = _ToBase(16);
                        break;
                    }
                case "frombase":
                    {
                        Value = FromBase(Arguments);
                        break;
                    }
                case "frombinary":
                    {
                        Value = FromBase(Arguments, 2);
                        break;
                    }
                case "fromoct":
                    {
                        Value = FromBase(Arguments, 8);
                        break;
                    }
                case "fromhex":
                    {
                        Value = FromBase(Arguments, 16);
                        break;
                    }*/
                case ">":
                case "op>":
                case "operator>":
                case "mt":
                case "gt":
                case "greater":
                case "greaterthan":
                case "more":
                case "morethan":
                case "isgreater":
                case "isgreaterthan":
                case "ismore":
                case "ismorethan":
                case ">=":
                case "op>=":
                case "operator>=":
                case "mte":
                case "gte":
                case "greaterorequal":
                case "greaterthanorequal":
                case "moreorequal":
                case "morethanorequal":
                case "isgreaterorequal":
                case "isgreaterthanorequal":
                case "ismoreorequal":
                case "ismorethanorequal":
                case "<":
                case "op<":
                case "operator<":
                case "lt":
                case "less":
                case "lessthan":
                case "isless":
                case "islessthan":
                case "<=":
                case "op<=":
                case "operator<=":
                case "lte":
                case "lessorequal":
                case "lessthanorequal":
                case "islessorequal":
                case "islessthanorequal":
                case "<>":
                case "!=":
                case "op<>":
                case "op!=":
                case "operator<>":
                case "operator!=":
                case "ne":
                case "isnotequal":
                case "notequal":
                case "==":
                case "op==":
                case "operator==":
                case "isequal":
                case "equal":
                case "eq":
                case "compare":
                    {
                        Value = _Compare(Name, Arguments);
                        break;
                    }
            }

            return Value;
        }

        /// <summary>
        /// Get the argument/ktValue as an float (takes in account strict typing etc)
        /// </summary>
        /// <param name="Arg">The argument</param>
        /// <returns></returns>
        private float GetAsFloat(ktValue Arg)
        {
            // Was we given a ktFloat??
            if (Arg.Type == "ktFloat")
            {
                // Get the value directly from the ktFloat object
                return ((ktFloat)Arg.Value).m_value;
            }
            // We got something else
            else
            {
                // Is this declared as a hard/strict type
                if (m_HardType)
                {
                    throw new ktError("ktFloat::Assign: Cant make '" + Arg.Type + "' into a (hard) ktFloat!", ktERR.WRONGTYPE);
                }
                // Not hard?
                else
                {
                    // Try to convert it to a float (Use the ToFloat of the argument value)
                    return Arg.ToFloat();
                }
            }
        }

        /// <summary>
        /// Assign a value to the float
        /// </summary>
        /// <param name="Arguments">The value to assign</param>
        /// <returns>ktValue with the current object</returns>
        public ktValue Assign(ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            // Can't assign nothing..
            if ((Arguments.IsEmpty()) || ((ktValue)Arguments.First.Node.Value).IsNull())
            {
                throw new ktError("Can't assign nothing (null) to a float!", ktERR.NOTDEF);
            }

            // Store the first value given as argument (ignore the rest)
            m_value = GetAsFloat((ktValue)Arguments.First.Node.Value);

            // Return this object wrapped in a ktValue
            return new ktValue(this.Name, "ktFloat", this, m_HardType, false);
        }

        /// <summary>
        /// Add values to the float (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to add to the float</param>
        /// <returns>The sum of all the arguments and the internal float</returns>
        private ktValue _Add(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            float res = 0.0f;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't add nothing (null) to a float!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Try to convert the argument to a float and add it to the sum
                res += GetAsFloat((ktValue)L.Node.Value);
            }
            // Add the current sum to the internal float, create a new ktFloat and wrap it in a ktValue
            Value = new ktValue("return", "ktFloat", new ktFloat(m_value + res), false, true);

            // Return the result
            return Value;
        }

        /// <summary>
        /// Add values to the float (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to add to the float</param>
        /// <returns>The sum of all the arguments and the internal float</returns>
        private ktValue Add(ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't add nothing (null) to a float!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Try to convert the argument to a float and add it to the sum
                m_value += GetAsFloat((ktValue)L.Node.Value);
            }
            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktFloat", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Increase the value of the object with 1
        /// </summary>
        /// <returns>This</returns>
        public ktValue Increase()
        {
            // Increase the value
            m_value++;
            // Wrap this object in a ktValue and return it
            return new ktValue("return", "ktFloat", this, m_HardType, true);
        }


        /// <summary>
        /// Subtract values from the float (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to subtract from the float</param>
        /// <returns>The difference of all the arguments and the internal float</returns>
        public ktValue _Subtract(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            float res = m_value;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't subtract nothing (null) to an float!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Subtract the current argument
                res -= GetAsFloat((ktValue)L.Node.Value);
            }
            // Create a new ktFloat and wrap it in a ktValue
            Value = new ktValue("return", "ktFloat", new ktFloat(res), true, true);

            return Value;
        }
        /// <summary>
        /// Subtract values from the float (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to subtract from the float</param>
        /// <returns>The difference of all the arguments and the internal float</returns>
        public ktValue Subtract(ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't subtract nothing (null) to an float!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Subtract the current argument
                m_value -= GetAsFloat((ktValue)L.Node.Value);
            }

            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktFloat", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Decrease the value of the object with 1
        /// </summary>
        /// <returns>This</returns>
        public ktValue Decrease()
        {
            // Decrease the value
            m_value--;
            // Wrap this object in a ktValue and return i
            return new ktValue("return", "ktFloat", this, m_HardType, true);
        }

        /// <summary>
        /// Multiply values with the float (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to multiply with the float</param>
        /// <returns>The product of all the arguments and the internal float</returns>
        public ktValue _Multiply(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            float res = m_value;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't multiply nothing (null) to a float!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Multiply with the current argument
                res *= GetAsFloat((ktValue)L.Node.Value);
            }
            // Create a new ktFloat and wrap it in a ktValue
            Value = new ktValue("return", "ktFloat", new ktFloat(res), true, true);

            return Value;
        }
        /// <summary>
        /// Multiply values with the float (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to multiply with the float</param>
        /// <returns>The product of all the arguments and the internal float</returns>
        public ktValue Multiply(ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't multiply nothing (null) with a float!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                // Multiply with the current argument
                m_value *= GetAsFloat((ktValue)L.Node.Value);
            }
            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktFloat", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Divide values with the float (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to divide the float with</param>
        /// <returns>The quotient of all the arguments and the internal float</returns>
        public ktValue _Divide(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            float res = m_value;
            float a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't divide an float with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an float
                a = GetAsFloat((ktValue)L.Node.Value);
                // Check so it isn't zero
                if (a == 0)
                {
                    // Oops, can't do that!!!
                    throw new ktError("You can't divide by zero!", ktERR.DIV_BY_ZERO);
                }
                // Divide with the current argument
                res /= a;
            }
            // Create a new ktFloat and wrap it in a ktValue
            Value = new ktValue("return", "ktFloat", new ktFloat(res), true, true);

            return Value;
        }
        /// <summary>
        /// Divide values with the float (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to divide the float with</param>
        /// <returns>The quotient of all the arguments and the internal float</returns>
        public ktValue Divide(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            ktValue Arg = ktValue.Null;
            float a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't divide an float with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an float
                a = GetAsFloat((ktValue)L.Node.Value);
                // Check so it isn't zero
                if (a == 0)
                {
                    // Oops, can't do that!!!
                    throw new ktError("You can't divide by zero!", ktERR.DIV_BY_ZERO);
                }
                // Divide with the current argument
                m_value /= a;
            }
            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktFloat", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Calculate the modulus of the float and the arguments (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to multiply with the float</param>
        /// <returns>The product of all the arguments and the internal float</returns>
        public ktValue _Modulus(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            ktValue Arg = ktValue.Null;
            float res = m_value;
            float a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't divide an float with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an float
                a = GetAsFloat((ktValue)L.Node.Value);
                // Do modulus with the current argument
                res %= a;
            }
            // Create a new ktFloat and wrap it in a ktValue
            Value = new ktValue("return", "ktFloat", new ktFloat(res), true, true);

            return Value;
        }
        /// <summary>
        /// Calculate the modulus of the float and the arguments (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to multiply with the float</param>
        /// <returns>The product of all the arguments and the internal float</returns>
        public ktValue Modulus(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            ktValue Arg = ktValue.Null;
            float a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't divide a float with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an float
                a = GetAsFloat((ktValue)L.Node.Value);
                // Do modulus with the current argument
                m_value %= a;
            }
            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktFloat", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Calculate the value of the float raised to the power of the arguments (doesn't change the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to raise the float with</param>
        /// <returns>The value of the internal float to the power of the arguments</returns>
        public ktValue _Power(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            float res = m_value;
            float a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't raise a float with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an float
                a = GetAsFloat((ktValue)L.Node.Value);
                // Raise it to the power of the argument
                res = (int)Math.Pow((double)res, (double)a);
            }
            // Create a new ktFloat and wrap it in a ktValue
            Value = new ktValue("return", "ktFloat", new ktFloat(res), true, true);

            return Value;
        }
        /// <summary>
        /// Calculate the value of the float raised to the power of the arguments (changes the value of the object)
        /// </summary>
        /// <param name="Arguments">A list of values to raise the float with</param>
        /// <returns>The value of the internal float to the power of the arguments</returns>
        public ktValue Power(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            float a = 1;

            // Check so we actually got some arguments
            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't raise a float with nothing (null)!", ktERR.NOTDEF);
            }

            // Go through the list of arguments
            foreach (ktList L in Arguments)
            {
                // If there's nothing in this argument
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                // Get the argument as an float
                a = GetAsFloat((ktValue)L.Node.Value);
                // Raise it to the power of the argument
                m_value = (int)Math.Pow((double)m_value, (double)a);
            }
            // Wrap this object in a ktValue ...
            Value = new ktValue("return", "ktFloat", this, m_HardType, true);

            // ... and return it
            return Value;
        }

        /// <summary>
        /// Round the float
        /// </summary>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        private ktValue Round(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            float res = m_value;
            int precision = 0;

            // Check if we got some arguments
            if (!Arguments.IsEmpty())
            {
                ktValue arg = (ktValue)Arguments.FirstNode.Value;
                if (!arg.IsNull())
                {
                    //ktClass obj = (ktClass)arg.Value;
                    if (arg.Type == "ktInt")
                    {
                        precision = arg.ToInt();
                    }
                    else
                    {
                        throw new ktError("ktFloat::Round() expected an integer as argument but got a '" + arg.Type + "'!", ktERR.WRONGTYPE);
                    }
                }
            }

            res = (float)Math.Round(res, precision);
            // Create a new ktFloat and wrap it in a ktValue
            Value = new ktValue("return", "ktFloat", new ktFloat(res), true, true);

            return Value;
        }

       /* /// <summary>
        /// Convert from decimal to another base
        /// </summary>
        /// <param name="tobase">The base to convert from</param>
        /// <returns></returns>
        public ktValue _ToBase(int tobase = 2)
        {
            int a = m_value 
            // Convert the value
            ktString str = Convert.ToString(m_value, tobase);
            // Create a new ktString and wrap it in a ktValue 
            return new ktValue("return", "ktString", kacTalk.Main.MakeObjectOf("ktString", str), true, true);
        }
        /// <summary>
        /// Convert from decimal to another base
        /// </summary>
        /// <param name="tobase">The base to convert from (We only use the first argument in the list)</param>
        /// <returns></returns>
        public ktValue ToBase(ktList Arguments)
        {
            ktValue Arg = ktValue.Null;
            int tobase = 0;

            // If we didn't get any arguments ...
            if (Arguments.IsEmpty())
            {
                // ... we use base 2 as a default value
                tobase = 2;
            }
            // We got arguments
            else
            {
                // Get the first value
                Arg = (ktValue)Arguments.First.Node.Value;
                // Convert the argument to an int
                tobase = Arg.ToInt();
            }

            // Convert
            return _ToBase(tobase);
        }*/


        /// <summary>
        /// Compare the float with the given value
        /// </summary>
        /// <param name="op">The operator to use for the comparison</param>
        /// <param name="val">The value to compare with</param>
        /// <returns>A value representing the comparison</returns>
        public override int Compare(ktString op, ktValue val)
        {
            int ret = 0;
            // Get the value as a float
            float fVal = GetAsFloat(val);

            // Check which operator we should use
            switch (op.AsLower())
            {
                case ">":
                case "op>":
                case "operator>":
                case "mt":
                case "gt":
                case "greater":
                case "greaterthan":
                case "more":
                case "morethan":
                case "isgreater":
                case "isgreaterthan":
                case "ismore":
                case "ismorethan":
                    {
                        ret = (m_value > fVal) ? 1 : 0;
                        break;
                    }
                case ">=":
                case "op>=":
                case "operator>=":
                case "mte":
                case "gte":
                case "greaterorequal":
                case "greaterthanorequal":
                case "moreorequal":
                case "morethanorequal":
                case "isgreaterorequal":
                case "isgreaterthanorequal":
                case "ismoreorequal":
                case "ismorethanorequal":
                    {
                        ret = (m_value >= fVal) ? 1 : 0;
                        break;
                    }
                case "<":
                case "op<":
                case "operator<":
                case "lt":
                case "less":
                case "lessthan":
                case "isless":
                case "islessthan":
                    {
                        ret = (m_value < fVal) ? 1 : 0;
                        break;
                    }
                case "<=":
                case "op<=":
                case "operator<=":
                case "lte":
                case "lessorequal":
                case "lessthanorequal":
                case "islessorequal":
                case "islessthanorequal":
                    {
                        ret = (m_value <= fVal) ? 1 : 0;
                        break;
                    }
                case "<>":
                case "!=":
                case "op<>":
                case "op!=":
                case "operator<>":
                case "operator!=":
                case "ne":
                case "isnotequal":
                case "notequal":
                    {
                        ret = (m_value != fVal) ? 1 : 0;
                        break;
                    }
                case "==":
                case "op==":
                case "operator==":
                case "isequal":
                case "equal":
                case "eq":
                    {
                        ret = (m_value == fVal) ? 1 : 0;
                        break;
                    }
                case "compare":
                    {
                        ret = m_value.CompareTo(fVal);
                        break;
                    }
                default:
                    {
                        throw new ktError("Couldn't find the method '" +
                                          op + "' in class '" + m_Name + "'.", ktERR._404);
                    }
            }

            return ret;
        }

        /// <summary>
        /// Compare the float with the given value
        /// </summary>
        /// <param name="op">The operator to use for the comparison</param>
        /// <param name="val">The value to compare with (in a list)</param>
        /// <returns>A value representing the comparison</returns>
        public override int Compare(ktString op, ktList arguments)
        {
            // If there's exactly one argument
            if (arguments.Count == 1)
            {
                return Compare(op, (ktValue)arguments.FirstNode.Value);
            }
            // Can't compare to more than one right now!
            else
            {
                throw new ktError("Compare for more than 1 value is not implemented in '" + this.m_Name + "'!");
            }
        }

        /// <summary>
        /// Compare the float with the given value
        /// </summary>
        /// <param name="op">The operator to use for the comparison</param>
        /// <param name="val">The value to compare with (in a list)</param>
        /// <returns>A value representing the comparison (wrapped in a ktValue)</returns>
        public ktValue _Compare(ktString Name, ktList Arguments)
        {
            // Do the comparison
            int ret = Compare(Name, Arguments);

            // Wrap the result in a ktValue
            return new ktValue("return", "ktInt", new ktInt(ret), false, true);
        }



        public override ktClass CreateObject(ktString Value)
        {
            return new ktFloat(Value.ToFloat());
        }
        public override ktClass CreateObject(ktValue Value)
        {
            return new ktFloat(GetAsFloat(Value));
        }
        public override ktClass CreateObject(object Value)
        {
            if (Value is float)
            {
                return new ktFloat((float)Value);
            }
            if ((Value is double) || (Value is Double))
            {
                return new ktFloat(Convert.ToSingle(Value));
            }
            else if ((Value is int) || (Value is Int32))
            {
                return new ktFloat((int)Value);
            }
            else
            {
                return new ktFloat((ktFloat)Value);
            }
        }

        public override ktClass CreateObject()
        {
            return new ktFloat();
        }
        public override string ToString()
        {
            return m_value.ToString(new CultureInfo("en-US"));
        }
        public override string Export(bool AsClass)
        {
            return ToString();
        }

        protected float m_value;
    }
}
