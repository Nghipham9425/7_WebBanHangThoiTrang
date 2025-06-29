using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Do_anNhom7.Models.ViewModel
{
    [Serializable] //Thuộc tính này cho phép đối tượng Cart có thể được tuần tự hóa 
    public class Cart
    {
        public List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items => items;
        //Thuộc tính này cung cấp một cách truy cập danh sách items dưới dạng IEnumerable (chỉ đọc)
        public void AddItem(int productId, string productImage, string productName, decimal unitPrice, int quantity, string category)
        {
            //Dùng FirstOrDefault để tìm kiếm xem sản phẩm đã có trong giỏ (dựa trên ProductID).
            var existingItem = items.FirstOrDefault(i => i.ProductID == productId);
            if (existingItem == null)
            {
                items.Add(new CartItem
                {
                    ProductID = productId,
                    ProductImage = productImage,
                    ProductName = productName,
                    UnitPrice = unitPrice,
                    Quantity = quantity
                });
            }
            else
            {
                existingItem.Quantity += quantity;
            }
        }

        public void RemoveItem(int productId)
        {
            items.RemoveAll(i => i.ProductID == productId);
        }

        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }
        public void Clear()
        {
            items.Clear();
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }
    }
}