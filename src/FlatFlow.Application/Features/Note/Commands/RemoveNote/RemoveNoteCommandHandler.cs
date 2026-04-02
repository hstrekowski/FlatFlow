using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Note.Commands.RemoveNote;

public class RemoveNoteCommandHandler : IRequestHandler<RemoveNoteCommand, Unit>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RemoveNoteCommandHandler> _logger;

    public RemoveNoteCommandHandler(
        IFlatRepository flatRepository,
        ITenantRepository tenantRepository,
        ICurrentUserService currentUserService,
        ILogger<RemoveNoteCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _tenantRepository = tenantRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Unit> Handle(RemoveNoteCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithNotesAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        var note = flat.Notes.FirstOrDefault(n => n.Id == request.NoteId)
            ?? throw new NotFoundException(nameof(Domain.Entities.Note), request.NoteId);

        var currentTenant = await _tenantRepository.GetByUserIdAndFlatIdAsync(
            _currentUserService.UserId, request.FlatId, cancellationToken)
            ?? throw new ForbiddenException("You are not a tenant in this flat.");

        if (!currentTenant.IsOwner && note.AuthorId != currentTenant.Id)
            throw new ForbiddenException("You can only remove your own notes.");

        flat.RemoveNote(request.NoteId);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Note {NoteId} removed from Flat {FlatId}", request.NoteId, flat.Id);

        return Unit.Value;
    }
}
