using UnityEngine;

public static class UIUtil
{
    /// <summary>
    /// 월드 오브젝트의 UI상 위치를 리턴 (화면 밖으로 나가지 않도록 보정 포함)
    /// </summary>
    /// <param name="worldCam">월드 오브젝트를 렌더링하는 카메라</param>
    /// <param name="uiCam">UI를 렌더링하는 카메라</param>
    /// <param name="worldTf">월드 오브젝트의 Transform</param>
    /// <param name="parentRt">UI가 속한 부모 RectTransform</param>
    /// <param name="padding">화면 밖으로 나가는 경우 추가 여백</param>
    /// <returns>UI상 위치</returns>
    public static Vector2 GetUIPosFromWorldTf(Camera worldCam, Camera uiCam, Transform worldTf, RectTransform parentRt)
    {
        // 월드 오브젝트의 위치를 화면 좌표로 변환
        Vector3 screenPos = worldCam.WorldToScreenPoint(worldTf.position);

        // 카메라 뒤쪽에 있으면 기본값 반환
        //if (screenPos.z < 0) screenPos.z = 1;

        // UI 카메라 기준으로 스크린 좌표를 UI 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRt, screenPos, uiCam, out Vector2 uiPos);

        return uiPos;
    }

    /// <summary>
    /// UI 요소가 화면 밖으로 나가지 않도록 보정
    /// </summary>
    public static Vector2 ClampToScreen(RectTransform parentRt, Vector2 uiPos, Vector2 padding)
    {
        Vector3[] corners = new Vector3[4];
        parentRt.GetWorldCorners(corners);

        // 부모 RectTransform의 크기 가져오기
        Vector2 min = parentRt.rect.min;
        Vector2 max = parentRt.rect.max;

        // UI 좌표 보정
        if (uiPos.x < min.x) uiPos.x = min.x + padding.x;
        if (uiPos.x > max.x) uiPos.x = max.x - padding.x;
        if (uiPos.y < min.y) uiPos.y = min.y + padding.y;
        if (uiPos.y > max.y) uiPos.y = max.y - padding.y;

        return uiPos;
    }

    /// <summary>
    /// UI 위치가 화면 밖인지
    /// </summary>
    /// <param name="parentRt"></param>
    /// <param name="uiPos"></param>
    /// <returns></returns>
    public static bool IsInScreen(RectTransform parentRt, Vector2 uiPos)
    {
        Vector3[] corners = new Vector3[4];
        parentRt.GetWorldCorners(corners);

        // 부모 RectTransform의 크기 가져오기
        Vector2 min = parentRt.rect.min;
        Vector2 max = parentRt.rect.max;

        // UI 좌표 보정
        if (uiPos.x < min.x) return false;
        if (uiPos.x > max.x) return false;
        if (uiPos.y < min.y) return false;
        if (uiPos.y > max.y) return false;

        return true;
    }
}
