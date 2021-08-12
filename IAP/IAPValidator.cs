using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPValidator : MonoBehaviour {
	public UnityEvent OnValidPurchaseSuccessful;
	public IAPButton IAPButton;

	private void Start() {
		IAPButton.onPurchaseComplete.AddListener((e) => ProcessPurchase(e));
	}

	public PurchaseProcessingResult ProcessPurchase(Product e) {
		bool validPurchase = true; // Presume valid for platforms with no R.V.

		// Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
		// Prepare the validator with the secrets we prepared in the Editor
		// obfuscation window.
		var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
			AppleTangle.Data(), Application.identifier);

		try {
			// On Google Play, result has a single product ID.
			// On Apple stores, receipts contain multiple products.
			var result = validator.Validate(e.receipt);
			// For informational purposes, we list the receipt(s)
			Debug.Log("Receipt is valid. Contents:");
			foreach (IPurchaseReceipt productReceipt in result) {
				Debug.Log(productReceipt.productID);
				Debug.Log(productReceipt.purchaseDate);
				Debug.Log(productReceipt.transactionID);
			}
		}
		catch (IAPSecurityException) {
			Debug.Log("Invalid receipt, not unlocking content");
			validPurchase = false;
		}
#endif
		Debug.Log("validPurchase: " + validPurchase);
		if (validPurchase) {
			// Unlock the appropriate content here.
			OnValidPurchaseSuccessful?.Invoke();
		}

		return PurchaseProcessingResult.Complete;
	}
}