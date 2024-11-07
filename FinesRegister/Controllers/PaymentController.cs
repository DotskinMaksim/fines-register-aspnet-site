using System.Security.Claims;
using FinesRegister.Models;
using FinesRegister.Data;
using FinesRegister.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;




namespace FinesRegister.Controllers;
[Route("[controller]")]
[ApiController]
public class PaymentController : Controller
{
   
    
    private readonly HttpClient _httpClient;

    public PaymentController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet("{sum}")]
    public async Task<IActionResult> MakePayment(string sum)
    {
        var paymentData = new
        {
            api_username = "e36eb40f5ec87fa2",
            account_name = "EUR3D1",
            amount = sum,
            order_reference = Math.Ceiling(new Random().NextDouble() * 999999),
            nonce = $"a9b7f7e7as{DateTime.Now}{new Random().NextDouble() * 999999}",
            timestamp = DateTime.Now,
            customer_url = "https://maksmine.web.app/makse"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(paymentData);        
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "ZTM2ZWI0MGY1ZWM4N2ZhMjo3YjkxYTNiOWUxYjc0NTI0YzJlOWZjMjgyZjhhYzhjZA==");

        var response = await client.PostAsync("https://igw-demo.every-pay.com/api/v4/payments/oneoff", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseContent);
            var paymentLink = jsonDoc.RootElement.GetProperty("payment_link");
            return Ok(paymentLink);
        }
        else
        {
            return BadRequest("Payment failed.");
        }
    }
}