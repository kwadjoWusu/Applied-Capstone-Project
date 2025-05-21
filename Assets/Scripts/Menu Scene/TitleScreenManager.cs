using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class TitleScreenManager : MonoBehaviour
{

    public static TitleScreenManager instance;
    [Header("Menu")]
    [SerializeField] GameObject titleScreenMainMenu;
    [SerializeField] GameObject titleScreenLoadMenu;

    [Header("Buttons")]
    [SerializeField] Button mainMenuNewGameButton;
    [SerializeField] Button loadMenuReturnButton;
    [SerializeField] Button mainMenuLoadGameButton;
    [SerializeField] Button deleteCharacterSlotPopUpConfirmButton;


    [Header("Pop Ups")]

    [SerializeField] GameObject noCharacterSlotsPopUp;
    [SerializeField] Button noCharacterSlotsOkayButton;
    [SerializeField] GameObject deleteCharacterSlotPopUp;

    [Header("Character Slots")]
    public CharacterSlots currentSelectedSlot = CharacterSlots.NO_SLOT;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.singletonInstance.AttemptToCreateNewGame();
    }

    public void OpenLoadGameMenu()
    {

        // Close Main Menu
        titleScreenMainMenu.SetActive(false);

        // Open Main Menu 
        titleScreenLoadMenu.SetActive(true);

        // select the return button first
        loadMenuReturnButton.Select();

    }

    public void CloseLoadGameMenu()
    {

        // Close Main Menu 
        titleScreenLoadMenu.SetActive(false);
        // Open Main Menu

        titleScreenMainMenu.SetActive(true);

        // select the Load button 
        mainMenuLoadGameButton.Select();

    }

    public void DisplayNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopUp.SetActive(true);
        noCharacterSlotsOkayButton.Select();

    }

    public void CloseNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopUp.SetActive(false);
        mainMenuNewGameButton.Select();
    }

    public void SelectCharacterSlot(CharacterSlots characterSlots)
    {
        currentSelectedSlot = characterSlots;
    }

    public void SelectNoSlot()
    {
        currentSelectedSlot = CharacterSlots.NO_SLOT;
    }

    public void AttemptToDeleteCharacterSlot()
    {

        if (currentSelectedSlot != CharacterSlots.NO_SLOT)
        {
            deleteCharacterSlotPopUp.SetActive(true);
            deleteCharacterSlotPopUpConfirmButton.Select();

        }

    }
    public void DeleteCharacterSlot()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        WorldSaveGameManager.singletonInstance.DeleteGame(currentSelectedSlot);
        titleScreenLoadMenu.SetActive(false);
        titleScreenLoadMenu.SetActive(true);
        loadMenuReturnButton.Select();
    }

    public void CloseDeleteCharacterPopUp()
    {

        deleteCharacterSlotPopUp.SetActive(false);
        loadMenuReturnButton.Select();

    }
}
