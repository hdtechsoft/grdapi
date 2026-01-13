using System;
using System.ComponentModel.DataAnnotations;

namespace grdApi.Models
{
    
    public class MerchDataRequest
    {
        public string? Category { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public bool IncludeOutOfStock { get; set; } = false;
    }

    public class MerchDataResponse
    {
        public int Count { get; set; }
        public string? error_code {get;set;}
        public string? error_desc {get;set;}
        public List<MerchItem> Items { get; set; } = new();
    }
    
    public class MerchItem
    {
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Price { get; set; }
        public bool InStock { get; set; }
    }

    public class clsiAttendanceDtl
    {
        public int course_id {get;set;}
        public int year_no {get;set;}
    }

    public class clsoAttendance
    {
        public string? error_code {get;set;}
        public string? error_desc{get;set;}
        public List<clsoAttendanceDtl> AttDtl { get; set; } = new();

    }

    public class clsoAttendanceDtl
    {
        public int course_id {get;set;}
        public int year_no {get;set;}
        public int sno {get;set;}
        public string? roll_no {get;set;}
        public string? stud_name {get;set;}
        public bool? presentabsent {get;set;}
        public bool? showflag {get;set;}
        public bool? enableflag {get;set;}
        public bool? showSaveMsg {get;set;}
    
    }



}