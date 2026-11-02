namespace MiniERP.Application.Assets.Dtos;

public class ImportAssetsResultDto
{
    public int TotalRows { get; set; }

    public int ImportedCount { get; set; }

    public int FailedCount { get; set; }

    public List<string> Errors { get; set; } = new();
}