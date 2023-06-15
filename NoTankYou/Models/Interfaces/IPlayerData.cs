using System.Linq;

namespace NoTankYou.Models.Interfaces;

public unsafe interface IPlayerData
{
    bool HasStatus(uint statusId);
    bool HasStatus(params uint[] statuses) => statuses.Any(HasStatus);
    bool MissingStatus(uint statusID) => !HasStatus(statusID);
    bool MissingStatus(params uint[] statuses) => !statuses.Any(HasStatus);
    bool HasPet();
    uint GetObjectId();
    byte* GetName();
    byte GetLevel();
    byte GetClassJob();
    protected bool HasClassJob(uint classJobId);
    bool MissingClassJob(uint classJobId) => !HasClassJob(classJobId);
    bool MissingClassJob(params uint[] classJobs) => !classJobs.Any(HasClassJob);
}