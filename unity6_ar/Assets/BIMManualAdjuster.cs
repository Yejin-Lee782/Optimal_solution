using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BIMManualAdjuster : MonoBehaviour
{
    [Header("���� ��� (BIM ��Ʈ)")]
    public Transform bimRoot;

    [Header("UI")]
    public GameObject panelContainer; // ��ư���� ���δ� �г�
    public TMP_Text debugText;

    [Header("���� ����")]
    public float positionStep = 0.1f; // 10cm
    public float rotationStep = 1f;   // 1��

    // ����/��ġ�� ���
    public void TogglePanel()
    {
        if (panelContainer != null)
            panelContainer.SetActive(!panelContainer.activeSelf);
    }

    // XYZ �̵� ����
    public void MoveXPlus() => AdjustPosition("x", +1);
    public void MoveXMinus() => AdjustPosition("x", -1);
    public void MoveYPlus() => AdjustPosition("y", +1);
    public void MoveYMinus() => AdjustPosition("y", -1);
    public void MoveZPlus() => AdjustPosition("z", +1);
    public void MoveZMinus() => AdjustPosition("z", -1);

    // Yaw, Pitch, Roll ȸ�� ����
    public void RotateYawPlus() => AdjustRotation("yaw", +1);
    public void RotateYawMinus() => AdjustRotation("yaw", -1);
    public void RotatePitchPlus() => AdjustRotation("pitch", +1);
    public void RotatePitchMinus() => AdjustRotation("pitch", -1);
    public void RotateRollPlus() => AdjustRotation("roll", +1);
    public void RotateRollMinus() => AdjustRotation("roll", -1);

    // ���� ��ġ �̵� �Լ�
    private void AdjustPosition(string axis, int direction)
    {
        if (bimRoot == null) return;

        float amount = positionStep * direction;
        Vector3 offset = Vector3.zero;

        switch (axis)
        {
            case "x": offset = new Vector3(amount, 0, 0); break;
            case "y": offset = new Vector3(0, amount, 0); break;
            case "z": offset = new Vector3(0, 0, amount); break;
        }

        bimRoot.position += offset;
        Log($"Moved {axis.ToUpper()} {amount}m �� {bimRoot.position}");
    }

    // ���� ȸ�� �Լ�
    private void AdjustRotation(string axis, int direction)
    {
        if (bimRoot == null) return;

        float angle = rotationStep * direction;
        Vector3 euler = Vector3.zero;

        switch (axis)
        {
            case "yaw": euler = new Vector3(0, angle, 0); break;
            case "pitch": euler = new Vector3(angle, 0, 0); break;
            case "roll": euler = new Vector3(0, 0, angle); break;
        }

        bimRoot.rotation *= Quaternion.Euler(euler);
        Log($"Rotated {axis.ToUpper()} {angle}�� �� {bimRoot.rotation.eulerAngles}");
    }

    // �α� ���
    private void Log(string msg)
    {
        Debug.Log(msg);
        if (debugText != null)
            debugText.text = msg;
    }
}
