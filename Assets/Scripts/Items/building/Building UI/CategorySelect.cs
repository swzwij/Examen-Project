using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategorySelect : MonoBehaviour
{
    [SerializeField] private Button _offensiveButton;
    [SerializeField] private Button _defensiveButton;

    [Space]
    [SerializeField] private Sprite _offensiveSelectedSprite;
    [SerializeField] private Sprite _offensiveUnselectedSprite;
    [SerializeField] private Sprite _defensiveSelectedSprite;
    [SerializeField] private Sprite _defensiveUnselectedSprite;

    [Space]
    [SerializeField] private GameObject _offensiveCategory;
    [SerializeField] private GameObject _defensiveCategory;

    private void Start()
    {
        _offensiveButton.image.sprite = _offensiveSelectedSprite;
        _defensiveButton.image.sprite = _defensiveUnselectedSprite;
    }

    private void OnEnable()
    {
        _offensiveButton.onClick.AddListener(OnOffensiveButtonClicked);
        _defensiveButton.onClick.AddListener(OnDefensiveButtonClicked);
    }

    private void OnDisable()
    {
        _offensiveButton.onClick.RemoveListener(OnOffensiveButtonClicked);
        _defensiveButton.onClick.RemoveListener(OnDefensiveButtonClicked);
    }

    private void OnOffensiveButtonClicked()
    {
        _offensiveButton.image.sprite = _offensiveSelectedSprite;
        _defensiveButton.image.sprite = _defensiveUnselectedSprite;

        _offensiveCategory.SetActive(true);
        _defensiveCategory.SetActive(false);
    }

    private void OnDefensiveButtonClicked()
    {
        _offensiveButton.image.sprite = _offensiveUnselectedSprite;
        _defensiveButton.image.sprite = _defensiveSelectedSprite;

        _offensiveCategory.SetActive(false);
        _defensiveCategory.SetActive(true);
    }
}
