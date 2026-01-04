using AutoMapper;
using InsightMed.Application.Auth.Models;
using InsightMed.Application.Auth.Services.Abstractions;
using InsightMed.Application.Common.Exceptions;
using MediatR;

namespace InsightMed.Application.Auth.Queries;

public sealed record GetAccountInfoQuery(string UserId) : IRequest<GetAccountInfoQueryResponse>;

public sealed class GetAccountInfoQueryHandler : IRequestHandler<GetAccountInfoQuery, GetAccountInfoQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;

    public GetAccountInfoQueryHandler(IMapper mapper, IAuthService authService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }
    
    public async Task<GetAccountInfoQueryResponse> Handle(GetAccountInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetUserByIdAsync(request.UserId)
            ?? throw new ResourceNotFoundException($"User with ID {request.UserId} not found");

        var response = _mapper.Map<GetAccountInfoQueryResponse>(user);

        return response;
    }
}