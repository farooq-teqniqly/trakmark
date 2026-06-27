using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// Groups the three outcome fields of a <see cref="Result"/> whose invariants are
/// enforced together: <see cref="Status"/> and the discipline type jointly govern
/// whether <see cref="Mark"/> and <see cref="Place"/> are present (place-only
/// disciplines require <see cref="Mark"/> to be <see langword="null"/> even when
/// the result is finished).
/// </summary>
internal sealed record ResultOutcome(ResultStatus Status, Performance? Mark, Placement? Place);
