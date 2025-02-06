using AutoMapper;
using Forum.Application.Common.DTOs;
using Forum.Domain.ValueObjects;
using Forum.Shared.EventBus;
using Forum.Shared.Events;

public class UserAggregate
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public EmailAddress Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CurrentJti { get; set; }
    public string? PreviousJti { get; set; }

    private List<IEvent> _domainEvents = new List<IEvent>();
    public IReadOnlyList<IEvent> DomainEvents => _domainEvents.AsReadOnly();

    public UserAggregate() { }

    public UserAggregate(CreateUserDTO dto, IMapper mapper)
    {
        mapper.Map(dto, this);
    }

    public void Activate()
    {
        if (IsActive) throw new InvalidOperationException("User is already active.");
        IsActive = true;
        AddDomainEvent(new UserActivatedEvent(Id, Username, Email.ToString()));
    }

    public void Deactivate()
    {
        if (!IsActive) throw new InvalidOperationException("User is already inactive.");
        IsActive = false;
        AddDomainEvent(new UserDeactivatedEvent(Id, Username, Email.ToString()));
    }

    private void AddDomainEvent(IEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void IncrementAccessFailedCount()
    {
        if (LockoutEnabled && LockoutEnd > DateTime.UtcNow)
        {
            throw new InvalidOperationException("User account is locked. Please try again later.");
        }

        AccessFailedCount++;

        if (AccessFailedCount >= 6)
        {
            LockoutEnabled = true;
            LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15);
        }
    }

    public void UnlockAccount()
    {
        LockoutEnabled = false;
        LockoutEnd = null;
        AccessFailedCount = 0;
        AddDomainEvent(new UserUnlockedEvent(Id, Username, Email.ToString()));
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrEmpty(newPasswordHash)) throw new ArgumentException("Password hash cannot be empty.");
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        if (EmailConfirmed) throw new InvalidOperationException("Email is already confirmed.");
        EmailConfirmed = true;
        AddDomainEvent(new EmailConfirmedEvent(Id, Username, Email.ToString()));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void UpdateJti(string newJti)
    {
        PreviousJti = CurrentJti;
        CurrentJti = newJti;
    }
}
