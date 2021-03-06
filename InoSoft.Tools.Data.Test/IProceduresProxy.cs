﻿namespace InoSoft.Tools.Data.Test
{
    public interface IProceduresProxy
    {
        Human[] GetHumans();

        int GetHumansCount();

        void AddHuman(long? id, string firstName, string lastName);

        Human GetHumanById(long id);

        Human GetHumanById(HumanId id);

        Human GetHumanById(HumanId? id);

        void GetHumanViaOutput(long id, out string firstName, out string lastName);

        void GetRandomHumanViaOutput(out long id, out string firstName, out string lastName);

        string ProcessText(string text);
    }
}