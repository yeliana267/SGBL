using System.Collections.Generic;

namespace SGBL.Application.ViewModels
{
    public class ReportCountCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Icon { get; set; } = "fa-chart-bar";
        public string ColorClass { get; set; } = "primary";
        public string? Description { get; set; }
    }

    public class ReportListItemViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Value { get; set; } = string.Empty;
        public string? BadgeClass { get; set; }
        public string? BadgeText { get; set; }
    }

    public class MonthlyReportItemViewModel
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AdminReportViewModel
    {
        public List<ReportCountCardViewModel> SummaryCards { get; set; } = new();
        public List<ReportListItemViewModel> RoleBreakdown { get; set; } = new();
        public List<ReportListItemViewModel> TopBooks { get; set; } = new();
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public int ReturnedThisMonth { get; set; }
        public int MonthlyLoans { get; set; }
    }

    public class LibrarianReportViewModel
    {
        public List<ReportCountCardViewModel> SummaryCards { get; set; } = new();
        public List<ReportListItemViewModel> UpcomingReturns { get; set; } = new();
        public List<ReportListItemViewModel> PendingPickups { get; set; } = new();
    }

    public class UserReportViewModel
    {
        public List<ReportCountCardViewModel> SummaryCards { get; set; } = new();
        public List<ReportListItemViewModel> ActiveLoans { get; set; } = new();
        public List<MonthlyReportItemViewModel> MonthlyActivity { get; set; } = new();
        public ReportListItemViewModel? NextReturn { get; set; }
    }
}