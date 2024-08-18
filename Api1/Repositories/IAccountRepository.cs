using Api1.Data;
using Microsoft.AspNetCore.Identity;

namespace Api1.Repositories
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> SignUpAsync(SignUpModel model);
        public Task<String> SignInAsync(SignInModel model);
    }
}
