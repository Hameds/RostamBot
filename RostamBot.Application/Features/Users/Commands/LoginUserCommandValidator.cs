using FluentValidation;

namespace RostamBot.Application.Features.Users.Commands
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUser>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("نام کاربری الزامی است")
                .EmailAddress()
                .WithMessage("نام کاربری باید ایمیل معتبری باشد");


            RuleFor(m => m.Password)
                .NotEmpty()
                .WithMessage("کلمه عبور الزامی است")
                .MinimumLength(10)
                .WithMessage("کلمه عبور حداقل ۱۰ کاراکتر است");
        }


    }
}
