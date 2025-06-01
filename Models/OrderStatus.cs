using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Models;
public enum OrderStatus
{
    Alındı,
    Hazırlanıyor,
    Hazır,
    [Display(Name = "Teslim Edildi")]
    TeslimEdildi
}