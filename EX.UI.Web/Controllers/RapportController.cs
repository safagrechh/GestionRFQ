using System;
using System.Text;
using EX.Core.Domain;
using EX.Core.Services;
using EX.UI.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RapportController : ControllerBase
    {
        private readonly IService<RFQ> _rfqService;
        private readonly IService<Client> _clientService;
        private readonly IService<MarketSegment> _segmentService;
        private readonly IService<Rapport> _rapportService;
        private readonly IActionHistoryLogger _actionHistoryLogger;

        public RapportController(
            IService<RFQ> rfqService,
            IService<Client> clientService,
            IService<MarketSegment> segmentService,
            IService<Rapport> rapportService,
            IActionHistoryLogger actionHistoryLogger)
        {
            _rfqService = rfqService;
            _clientService = clientService;
            _segmentService = segmentService;
            _rapportService = rapportService;
            _actionHistoryLogger = actionHistoryLogger;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(claim) && int.TryParse(claim, out userId))
            {
                return true;
            }
            userId = 0;
            return false;
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Validateur,Lecteur")]
        public IActionResult Generate([FromBody] GenerateReportRequest request)
        {
            if (request == null)
            {
                return BadRequest("Payload manquant.");
            }

            if (request.StartDate.HasValue && request.EndDate.HasValue && request.EndDate < request.StartDate)
            {
                return BadRequest("La date de fin doit être après la date de début.");
            }

            var rfqs = _rfqService.GetAll().AsEnumerable();

            if (request.StartDate.HasValue)
            {
                var start = request.StartDate.Value.Date;
                rfqs = rfqs.Where(r => r.DateCreation >= start);
            }
            if (request.EndDate.HasValue)
            {
                var inclusiveEnd = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                rfqs = rfqs.Where(r => r.DateCreation <= inclusiveEnd);
            }
            if (request.MarketSegmentIds != null && request.MarketSegmentIds.Length > 0)
            {
                var set = new HashSet<int>(request.MarketSegmentIds);
                rfqs = rfqs.Where(r => r.MarketSegmentId.HasValue && set.Contains(r.MarketSegmentId.Value));
            }
            if (request.ClientIds != null && request.ClientIds.Length > 0)
            {
                var set = new HashSet<int>(request.ClientIds);
                rfqs = rfqs.Where(r => set.Contains(r.ClientId));
            }
            // Filter by Statut (Win/Loss) if provided
            if (request.Statuts != null && request.Statuts.Length > 0)
            {
                var statutSet = new HashSet<Statut>(
                    request.Statuts
                        .Select(s => Enum.TryParse<Statut>(s, true, out var v) ? v : (Statut?)null)
                        .Where(v => v.HasValue)
                        .Select(v => v!.Value));

                if (statutSet.Count > 0)
                    rfqs = rfqs.Where(r => r.Statut.HasValue && statutSet.Contains(r.Statut.Value));
            }

            var ordered = rfqs.OrderByDescending(r => r.DateCreation).ToList();

            // Si aucune donnée ne correspond aux filtres, renvoyer une erreur claire
            if (ordered.Count == 0)
            {
                return BadRequest("Aucune donnée disponible pour les filtres sélectionnés.");
            }

            // Build lookup maps for names
            var clientMap = _clientService.GetAll().ToDictionary(c => c.Id, c => c.Nom);
            var segMap = _segmentService.GetAll().ToDictionary(s => s.Id, s => s.Nom);

            // Préparer un résumé des filtres pour l'en-tête du PDF
            string? startStr = request.StartDate?.ToString("yyyy-MM-dd");
            string? endStr = request.EndDate?.ToString("yyyy-MM-dd");
            var parts = new List<string>();
            if (startStr != null || endStr != null)
                parts.Add($"Période: {startStr ?? ""} → {endStr ?? ""}");
            if (request.MarketSegmentIds != null && request.MarketSegmentIds.Length > 0)
                parts.Add($"Segments: {string.Join(",", request.MarketSegmentIds)}");
            if (request.ClientIds != null && request.ClientIds.Length > 0)
                parts.Add($"Clients: {string.Join(",", request.ClientIds)}");
            if (request.Statuts != null && request.Statuts.Length > 0)
                parts.Add($"Statuts: {string.Join(",", request.Statuts)}");
            var filterSummary = parts.Count > 0 ? string.Join(" | ", parts) : "Aucun filtre";

            var format = (request.Format ?? "csv").ToLowerInvariant();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

            if (format == "pdf")
            {
                QuestPDF.Settings.License = LicenseType.Community;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(40);
                        page.DefaultTextStyle(TextStyle.Default.Size(10));

                        page.Header()
                            .Column(col =>
                            {
                                col.Item().Text($"Rapport RFQ – {DateTime.UtcNow:yyyy-MM-dd}")
                                    .SemiBold().FontSize(18);
                                col.Item().Text(filterSummary)
                                    .FontSize(10).Italic().FontColor(Colors.Grey.Darken2);
                            });

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(75); // CQ
                                columns.RelativeColumn(3);  // QuoteName (wider)
                                columns.RelativeColumn(2);  // Client
                                columns.RelativeColumn(2);  // Segment
                                columns.ConstantColumn(85); // Statut
                                columns.ConstantColumn(100); // Date
                            });

                            // Styles
                            Func<IContainer, IContainer> CellStyle = c => c
                                .PaddingVertical(4).PaddingHorizontal(6)
                                .BorderBottom(1).BorderColor(Colors.Grey.Lighten3);

                            Func<IContainer, IContainer> HeaderCellStyle = c => c
                                .Background(Colors.Grey.Lighten3)
                                .PaddingVertical(6).PaddingHorizontal(6);

                            table.Header(header =>
                            {
                                HeaderCellStyle(header.Cell()).Text("CQ").SemiBold();
                                HeaderCellStyle(header.Cell()).Text("QuoteName").SemiBold();
                                HeaderCellStyle(header.Cell()).Text("Client").SemiBold();
                                HeaderCellStyle(header.Cell()).Text("Segment").SemiBold();
                                HeaderCellStyle(header.Cell()).Text("Statut").SemiBold().AlignCenter();
                                HeaderCellStyle(header.Cell()).Text("DateCreation").SemiBold().AlignCenter();
                            });

                            var index = 0;
                            foreach (var r in ordered)
                            {
                                var bg = (index++ % 2 == 0) ? Colors.White : Colors.Grey.Lighten5;

                                var clientNom = clientMap.TryGetValue(r.ClientId, out var cn) ? cn : string.Empty;
                                var segNom = (r.MarketSegmentId.HasValue && segMap.TryGetValue(r.MarketSegmentId.Value, out var sn)) ? sn : string.Empty;
                                var statutStr = r.Statut?.ToString() ?? string.Empty;
                                var dateStr = r.DateCreation.ToString("yyyy-MM-dd");

                                CellStyle(table.Cell().Background(bg)).Text(r.CQ.ToString());
                                CellStyle(table.Cell().Background(bg)).Text(r.QuoteName ?? string.Empty);
                                CellStyle(table.Cell().Background(bg)).Text(clientNom);
                                CellStyle(table.Cell().Background(bg)).Text(segNom);
                                CellStyle(table.Cell().Background(bg)).AlignCenter().Text(statutStr);
                                CellStyle(table.Cell().Background(bg)).AlignCenter().Text(dateStr);
                            }
                        });

                        page.Footer().Row(row =>
                        {
                            row.RelativeColumn().Text($"Total: {ordered.Count} RFQ")
                                .FontSize(10).FontColor(Colors.Grey.Darken2);
                            row.ConstantColumn(180).AlignRight().Text(text =>
                            {
                                text.Span("Page ");
                                text.CurrentPageNumber();
                                text.Span(" / ");
                                text.TotalPages();
                            });
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                var pdfName = $"rapport_rfq_{timestamp}.pdf";

                if (TryGetCurrentUserId(out var userIdPdf))
                {
                    var rapport = new Rapport
                    {
                        CheminFichier = pdfName,
                        DateCreation = DateTime.UtcNow,
                        UserId = userIdPdf
                    };
                    _rapportService.Add(rapport);

                    _actionHistoryLogger.LogAction(
                        "REPORT_GENERATED",
                        "RAPPORT",
                        pdfName,
                        $"Rapport RFQ généré (PDF): {ordered.Count} lignes.",
                        userIdPdf);
                }

                return File(pdfBytes, "application/pdf", pdfName);
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("CQ;QuoteName;Client;Segment;Statut;DateCreation;Valide;Rejete;Brouillon");
                foreach (var r in ordered)
                {
                    var clientNom = clientMap.TryGetValue(r.ClientId, out var cn) ? cn : "";
                    var segNom = (r.MarketSegmentId.HasValue && segMap.TryGetValue(r.MarketSegmentId.Value, out var sn)) ? sn : "";
                    var statutStr = r.Statut?.ToString() ?? "";
                    var dateStr = r.DateCreation.ToString("yyyy-MM-dd");
                    sb.AppendLine(string.Join(";", new[]
                    {
                        r.CQ.ToString(),
                        (r.QuoteName ?? "").Replace(";", ","),
                        clientNom.Replace(";", ","),
                        segNom.Replace(";", ","),
                        statutStr,
                        dateStr,
                        r.Valide.ToString(),
                        r.Rejete.ToString(),
                        r.Brouillon.ToString()
                    }));
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                var fileName = $"rapport_rfq_{timestamp}.csv";

                if (TryGetCurrentUserId(out var userId))
                {
                    var rapport = new Rapport
                    {
                        CheminFichier = fileName,
                        DateCreation = DateTime.UtcNow,
                        UserId = userId
                    };
                    _rapportService.Add(rapport);

                    _actionHistoryLogger.LogAction(
                        "REPORT_GENERATED",
                        "RAPPORT",
                        fileName,
                        $"Rapport RFQ généré: {ordered.Count} lignes.",
                        userId);
                }

                return File(bytes, "text/csv", fileName);
            }
        }
    }

    public class GenerateReportRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int[]? MarketSegmentIds { get; set; }
        public int[]? ClientIds { get; set; }
        // Accept Win/Loss filters as strings to avoid enum converter requirements
        public string[]? Statuts { get; set; }
        public string? Format { get; set; } // Par défaut CSV
    }
}