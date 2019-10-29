
namespace Orikivo
{
    public static class VarBuilder // Handles creating variable IDs.
    {
        public static string CreateVarId(VarType type, VarGroup group, string name)
            => $"{type.ToString().ToLower()}:{group.ToString().ToLower()}:{OriFormat.Jsonify(name)}";
    }
}
