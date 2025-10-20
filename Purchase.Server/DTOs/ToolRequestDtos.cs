using System.ComponentModel.DataAnnotations;

namespace Purchase.Server.DTOs;

public class CreateToolRequestDto
{
    public int? ToolId { get; set; }
    
    public string? ToolName { get; set; }

    [Required(ErrorMessage = "الكمية المطلوبة مطلوبة")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية أكبر من صفر")]
    public int QuantityNeeded { get; set; }

    public int? WorkAreaId { get; set; }
    
    public string? WorkAreaName { get; set; }
    
    public string? RequesterName { get; set; }
    
    public string? Status { get; set; }

    [Required(ErrorMessage = "السبب مطلوب")]
    [StringLength(500, ErrorMessage = "يجب ألا يتجاوز السبب 500 حرف")]
    public string ReasonAr { get; set; } = string.Empty;

    [StringLength(500)]
    public string ReasonEn { get; set; } = string.Empty;
}

public class ToolRequestDto
{
    public int Id { get; set; }
    public int ToolId { get; set; }
    public string ToolNameAr { get; set; } = string.Empty;
    public string ToolNameEn { get; set; } = string.Empty;
    public int QuantityNeeded { get; set; }
    public int WorkAreaId { get; set; }
    public string WorkAreaNameAr { get; set; } = string.Empty;
    public string WorkAreaNameEn { get; set; } = string.Empty;
    public int RequesterId { get; set; }
    public string RequesterNameAr { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string ReasonAr { get; set; } = string.Empty;
    public string ReasonEn { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsInStock { get; set; }
    public int? QuantityFulfilled { get; set; }
    public DateTime? FulfilledDate { get; set; }
    public string? StockVerificationNotes { get; set; }
}

public class StockVerificationDto
{
    [Required]
    public int ToolRequestId { get; set; }

    [Required]
    public bool IsInStock { get; set; }

    public int? QuantityFulfilled { get; set; }

    [StringLength(500)]
    public string? StockVerificationNotes { get; set; }
}
