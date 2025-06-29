<script>
    // Hàm tính toán giá trị tạm tính
    function updateEstimatedPrice() {
        var quantity = parseInt(document.getElementById("quantityInput").value);
    var price = @Model.product.ProductPrice;  // Giá sản phẩm từ model
    var estimatedValue = quantity * price; // Tính tạm tính

    // Cập nhật giá trị tạm tính
    document.getElementById("estimatedPrice").innerText = estimatedValue.toLocaleString() + " VND";
    }

    // Gắn sự kiện thay đổi cho input số lượng
    document.getElementById("quantityInput").addEventListener("input", updateEstimatedPrice);
</script>
