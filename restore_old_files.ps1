# Restore old files from commit d15f74f
$commit = "d15f74f"
$files = @(
    ".gitignore",
    "FashionStore/Controllers/AccountController.cs",
    "FashionStore/Controllers/Api/ProductsApiController.cs",
    "FashionStore/Data/DatabaseInitializer.cs",
    "FashionStore/FashionStore.csproj",
    "FashionStore/Models/Entities/Product.cs",
    "FashionStore/Repositories/Implementations/UserRepository.cs",
    "FashionStore/Services/Implementations/ProductService.cs",
    "FashionStore/Views/Account/Login.cshtml",
    "FashionStore/Views/Account/Register.cshtml",
    "FashionStore/Views/Home/Index.cshtml",
    "FashionStore/Views/Product/Details.cshtml",
    "FashionStore/Views/Shared/_Layout.cshtml"
)

foreach ($file in $files) {
    Write-Host "Restoring $file..."
    git show "$commit`:$file" | Out-File -FilePath $file -Encoding UTF8
}

Write-Host "All files restored!"

