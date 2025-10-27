"""
DEBUGGING SCRIPT - Run this to diagnose module registration issues
"""

import sys
import os
sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'Modules'))

from Modules.message_broker import get_broker
from Modules import deauth, mitm

print("\n" + "="*60)
print("DEBUGGING MODULE REGISTRATION")
print("="*60)

# Step 1: Get broker
print("\n1. Getting broker instance...")
broker = get_broker()
print(f"   Broker instance: {broker}")
print(f"   Module queues (should be empty): {list(broker.module_queues.keys())}")

# Step 2: Create modules (this should register them)
print("\n2. Creating deauth module...")
deauth_mod = deauth.get_module()
print(f"   Deauth module: {deauth_mod}")
print(f"   Module name: {deauth_mod.module_name}")
print(f"   Module queues now: {list(broker.module_queues.keys())}")

print("\n3. Creating mitm module...")
mitm_mod = mitm.get_module()
print(f"   MITM module: {mitm_mod}")
print(f"   Module name: {mitm_mod.module_name}")
print(f"   Module queues now: {list(broker.module_queues.keys())}")

# Step 3: Check registration
print("\n4. Checking broker registration:")
for module_name, queue in broker.module_queues.items():
    print(f"   - {module_name}: queue={queue}, empty={queue.empty()}")

# Step 4: Try to manually route a message
print("\n5. Testing message routing manually...")
from Modules.message_broker import Message, MessageType

test_msg = Message(MessageType.COMMAND, "deauth", "get_status", None)
print(f"   Created test message: {test_msg.module}.{test_msg.action}")

# Check if module is in queues
if test_msg.module in broker.module_queues:
    print(f"   ✓ Module '{test_msg.module}' IS registered in broker")
    broker.module_queues[test_msg.module].put(test_msg)
    print(f"   ✓ Message placed in queue")
else:
    print(f"   ✗ Module '{test_msg.module}' NOT registered in broker")
    print(f"   Available modules: {list(broker.module_queues.keys())}")

print("\n" + "="*60)
print("DIAGNOSIS COMPLETE")
print("="*60)
print("\nIf you see 'NOT registered' above, there's an import/timing issue.")
print("If you see 'IS registered', the broker is working correctly.")
