using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Note.Commands.UpdateNote;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Unit>
{
    private readonly INoteRepository _noteRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateNoteCommandHandler> _logger;

    public UpdateNoteCommandHandler(
        INoteRepository noteRepository,
        ITenantRepository tenantRepository,
        ICurrentUserService currentUserService,
        ILogger<UpdateNoteCommandHandler> logger)
    {
        _noteRepository = noteRepository;
        _tenantRepository = tenantRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Note), request.NoteId);

        var currentTenant = await _tenantRepository.GetByUserIdAndFlatIdAsync(
            _currentUserService.UserId, note.FlatId, cancellationToken)
            ?? throw new ForbiddenException("You are not a tenant in this flat.");

        if (!currentTenant.IsOwner && note.AuthorId != currentTenant.Id)
            throw new ForbiddenException("You can only modify your own notes.");

        note.UpdateTitle(request.Title);
        note.UpdateContent(request.Content);

        await _noteRepository.UpdateAsync(note, cancellationToken);

        _logger.LogInformation("Note {NoteId} updated", note.Id);

        return Unit.Value;
    }
}
