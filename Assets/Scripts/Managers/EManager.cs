using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EManager
{
    //�Ŵ��� ������ �� �ѹ��� ���� - createInstance���� ȣ���ص� ����
    public void init();
    //Managers.Update�� �־ ���
    public void update_();
    //�Ŵ��� ������ ���, Inspecter���� �� �Լ����� ���� ������Ʈ �����Ͽ� ����Ұ�
    EManager createInstance();


}
