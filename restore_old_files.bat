@echo off
set GIT_PAGER=cat
git show d15f74f:.gitignore > .gitignore
git show d15f74f:FashionStore/Controllers/AccountController.cs > FashionStore/Controllers/AccountController.cs
git show d15f74f:FashionStore/Controllers/Api/ProductsApiController.cs > FashionStore/Controllers/Api/ProductsApiController.cs
git show d15f74f:FashionStore/Data/DatabaseInitializer.cs > FashionStore/Data/DatabaseInitializer.cs
git show d15f74f:FashionStore/FashionStore.csproj > FashionStore/FashionStore.csproj
git show d15f74f:FashionStore/Models/Entities/Product.cs > FashionStore/Models/Entities/Product.cs
git show d15f74f:FashionStore/Repositories/Implementations/UserRepository.cs > FashionStore/Repositories/Implementations/UserRepository.cs
git show d15f74f:FashionStore/Services/Implementations/ProductService.cs > FashionStore/Services/Implementations/ProductService.cs
git show d15f74f:FashionStore/Views/Account/Login.cshtml > FashionStore/Views/Account/Login.cshtml
git show d15f74f:FashionStore/Views/Account/Register.cshtml > FashionStore/Views/Account/Register.cshtml
git show d15f74f:FashionStore/Views/Home/Index.cshtml > FashionStore/Views/Home/Index.cshtml
git show d15f74f:FashionStore/Views/Product/Details.cshtml > FashionStore/Views/Product/Details.cshtml
git show d15f74f:FashionStore/Views/Shared/_Layout.cshtml > FashionStore/Views/Shared/_Layout.cshtml
echo All files restored from commit d15f74f!

