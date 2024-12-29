using Microsoft.AspNetCore.Mvc;
using SushiWebsite.Models;
using System.Linq;
using System.Text;
using System.Text.Json;
namespace SushiWebsite.Controllers
{
    public class MenuController : Controller
    {
        private readonly IConfiguration _configuration;
        public MenuController (IConfiguration configuration)
        {
            _configuration = configuration; 
        }
        // Danh sách món ăn (tạm thời đọc từ file JSON)
        private List<Dish> GetDishes()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/data/menu.json");
            string jsonData = System.IO.File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Dish>>(jsonData);
        }

        // Giỏ hàng tạm (lưu trong Session)
        private List<CartItem> Cart
        {
            get
            {
                var cart = HttpContext.Session.GetString("Cart");
                return cart != null ? JsonSerializer.Deserialize<List<CartItem>>(cart) : new List<CartItem>();
            }
            set
            {
                HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(value));
            }
        }

        public IActionResult Index()
        {
            var dishes = GetDishes().GroupBy(d => d.Category);

            // Tính tổng số lượng món ăn trong giỏ hàng
            int totalItemsInCart = Cart.Sum(item => item.Quantity);
            ViewBag.CartItemCount = totalItemsInCart;

            return View(dishes);

        }
        // Hiển thị giỏ hàng
        public IActionResult CartView()
        {
            return View(Cart);
        }

        // Xử lý thêm món vào giỏ hàng
        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var dish = GetDishes().FirstOrDefault(d => d.Id == id);
            if (dish != null)
            {
                var cart = Cart;
                var item = cart.FirstOrDefault(c => c.DishId == id);

                if (item == null)
                {
                    cart.Add(new CartItem { DishId = dish.Id, Name = dish.Name, Price = dish.Price, Quantity = 1 });
                }
                else
                {
                    item.Quantity++;
                }

                Cart = cart; // Cập nhật giỏ hàng
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GetCartItems()
        {
            var cart = Cart;
            return Json(cart);
        }

        // Xóa món ăn khỏi giỏ hàng
        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = Cart;
            var item = cart.FirstOrDefault(c => c.DishId == id);
            if (item != null)
            {
                cart.Remove(item);
                Cart = cart; // Cập nhật giỏ hàng
            }
            return RedirectToAction("CartView");
        }

        /////////////////
        // Đặt hàng và gửi thông báo qua Telegram
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string firstName, string lastName, string phoneNumber, DateTime datetimePicker)
        {
            // Kiểm tra thông tin đầu vào
            if (!IsValidOrder(firstName, lastName, phoneNumber, datetimePicker, out string validationError))
            {
                ModelState.AddModelError(string.Empty, validationError);
                return View("CartView", Cart); // Hiển thị lại giỏ hàng với lỗi
            }

            // Tạo tin nhắn đặt hàng
            string message = CreateOrderMessage(firstName, lastName, phoneNumber, datetimePicker, Cart);

            // Gửi tin nhắn qua Telegram
            await SendMessageToTelegram(message);

            // Reset giỏ hàng
            Cart = new List<CartItem>();

            return RedirectToAction("OrderConfirmation");
        }

        // Hiển thị xác nhận đặt hàng
        public IActionResult OrderConfirmation()
        {
            ViewBag.Message = "Your order has been placed successfully!";
            return View();
        }

        // Kiểm tra thông tin đơn hàng hợp lệ
        private bool IsValidOrder(string firstName, string lastName, string phoneNumber, DateTime datetimePicker, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(phoneNumber) ||
                datetimePicker == default)
            {
                errorMessage = "Please enter complete information.";
                return false;
            }

            if (!Cart.Any())
            {
                errorMessage = "Your cart is empty. Add items before placing the order.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        // Tạo nội dung tin nhắn đặt hàng
        private string CreateOrderMessage(string firstName, string lastName, string phoneNumber, DateTime datetimePicker, List<CartItem> cartItems)
        {
            var orderDetails = cartItems.Select(item => $"- {item.Name}: {item.Quantity} x {item.Price:C}");
            return $@"
New Order Received:
- First Name: {firstName}
- Last Name: {lastName}
- Phone Number: {phoneNumber}
- Dining Date and Time: {datetimePicker:dd/MM/yyyy HH:mm}

Order Details:
{string.Join("\n", orderDetails)}
Total Amount: {cartItems.Sum(item => item.Quantity * item.Price):C}
            ";
        }
        // Gửi thông báo qua Telegram
        private async Task SendMessageToTelegram(string message)
        {
            string telegramBotToken = _configuration["TelegramSettings:BotToken"];
            string chatId = _configuration["TelegramSettings:ChatId"];

            string url = $"https://api.telegram.org/bot{telegramBotToken}/sendMessage";
            using var client = new HttpClient();
            var payload = new
            {
                chat_id = chatId,
                text = message,
                parse_mode = "Markdown"
            };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            await client.PostAsync(url, content);
        }

       
       
    }
}
