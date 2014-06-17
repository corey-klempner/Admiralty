using UnityEngine;
using System.Collections;


public class Cargo
{ 

	public string name; 
	public int selectedQuantity; //Displayed selected quantity for trades and moving
	public int quantity;
	public int weight; //Weight of one item in tons
	public int totalWeight; //Weight of combined items in tons
	public int value = 10; 

	private string info; 
	
	public Cargo(string nm, int amt, int w){
		this.name = nm; 
		this.weight = w;
		this.add (amt);
	}
	
	public void add(int q){
		this.totalWeight = this.totalWeight + q * this.weight;
	
		this.quantity = this.quantity + q;  
	}
	
	public void remove(int q)
	{
		this.totalWeight -= q * this.weight;
		this.quantity -= q; 
	}
	
	public void removeSelected(){
		this.totalWeight -= this.getSelectedQuantity () * weight;
		this.quantity -= this.getSelectedQuantity (); 
	}
	
	public string getInfo()
	{
		return this.info;
	}
	public void setInfo(string info)
	{
		this.info = info;
	}
	
	/*
		 * Override the ToString to print object name
		 */
	public override string ToString()
	{
		return this.name;
	}
	
	/*
		 * Getters and Setter methods for cargo objects
		 */
	public int getSelectedQuantity()
	{
		return this.selectedQuantity;
	}
	
	public void setSelectedQuantity(int q)
	{
		this.selectedQuantity = Mathf.Clamp (q, 0, this.quantity);
	}
	
	public int getQuantity()
	{
		return this.quantity;
	}
	public void setQuantity(int x)
	{
		this.quantity = x;
	}

	public int getTotalWeight()
	{
		return this.totalWeight;
	}
}


