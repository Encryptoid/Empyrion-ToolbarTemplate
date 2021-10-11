# Empyrion-ToolbarTemplate

This mod allows you to save a template of your toolbar, and then load that template from items in your inventory. (Most useful when you die and pick up your stuff)

The "entry point" for the is ToolbarTemplate.cs

The following commands are made available, to be typed in the Server tab:

# !tt save
This will save the users current toolbar as a template file in the mods database folder.

# !tt
This will load the template from items in your inventory and current toolbar. Overflow should be handled. 
I picked a short command to make it quick and easy to type.
Note: ItemSlotIdx would not always work for me, so I reassign my own. Items will load in the correct order but always be squashed to the left to make room for overflow.

# Initial Setup
Please read the Initial Setup section here: https://github.com/Encryptoid/zucchini-empyrion
