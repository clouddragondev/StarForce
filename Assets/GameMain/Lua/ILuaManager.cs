using System.Collections;
using System.Collections.Generic;

public interface ILuaManager
{
    void Init();

    void DoString(string values);

    void Destory();
}
