using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EManager
{
    //매니저 생성시 단 한번만 실행 - createInstance에서 호출해도 무방
    public void init();
    //Managers.Update에 넣어서 사용
    public void update_();
    //매니저 생성시 사용, Inspecter사용시 본 함수에서 게임 오브젝트 생성하여 사용할것
    EManager createInstance();


}
