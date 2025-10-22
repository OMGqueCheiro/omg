using OMG.Core.Base;

namespace OMG.Core.Entities;

public class Cliente : Entity
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
}
