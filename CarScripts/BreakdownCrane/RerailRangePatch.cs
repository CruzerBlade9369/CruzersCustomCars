using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using DV;

namespace BreakdownCrane
{
    [HarmonyPatch(typeof(RerailController), "OnUpdate")]
    public static class RerailRangePatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 100f)
                {
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 200f);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}