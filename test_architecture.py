"""
Test script for the message broker architecture
Run this to verify everything is working correctly
"""

import sys
import os
import time
import json

# Add Modules directory to path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'Modules'))

from Modules.message_broker import Message, MessageType, get_broker
from Modules import deauth, mitm

class MockSocket:
    """Mock socket for testing without actual network connection"""
    def __init__(self):
        self.sent_messages = []
        self.receive_buffer = []
        
    def recv(self, size):
        if self.receive_buffer:
            data = self.receive_buffer.pop(0)
            return data.encode('utf-8')
        time.sleep(0.1)  # Simulate waiting
        return b''
    
    def sendall(self, data):
        msg = data.decode('utf-8').strip()
        if msg:
            self.sent_messages.append(msg)
            print(f"[MockSocket] Sent: {msg[:100]}")
    
    def send_command(self, module, action, data=None):
        """Simulate sending a command from C#"""
        msg = Message(MessageType.COMMAND, module, action, data)
        json_msg = msg.to_json() + '\n'
        self.receive_buffer.append(json_msg)
        print(f"[MockSocket] Queued command: {module}.{action}")

def test_broker_initialization():
    """Test 1: Broker initialization"""
    print("\n" + "="*60)
    print("TEST 1: Broker Initialization")
    print("="*60)
    
    broker = get_broker()
    print("✓ Broker instance created")
    
    # Register test modules
    queue1 = broker.register_module("test_module_1")
    queue2 = broker.register_module("test_module_2")
    print("✓ Modules registered")
    
    assert "test_module_1" in broker.module_queues
    assert "test_module_2" in broker.module_queues
    print("✓ Module queues created")
    
    print("\nTest 1 PASSED ✓")
    return broker

def test_module_initialization():
    """Test 2: Module initialization"""
    print("\n" + "="*60)
    print("TEST 2: Module Initialization")
    print("="*60)
    
    deauth_mod = deauth.get_module()
    print("✓ Deauth module created")
    
    mitm_mod = mitm.get_module()
    print("✓ MITM module created")
    
    # Start modules
    deauth_mod.start()
    print("✓ Deauth module started")
    
    mitm_mod.start()
    print("✓ MITM module started")
    
    time.sleep(0.5)  # Let modules initialize
    
    assert deauth_mod.running
    assert mitm_mod.running
    print("✓ Modules are running")
    
    print("\nTest 2 PASSED ✓")
    return deauth_mod, mitm_mod

def test_message_routing():
    """Test 3: Message routing"""
    print("\n" + "="*60)
    print("TEST 3: Message Routing")
    print("="*60)
    
    mock_socket = MockSocket()
    broker = get_broker()
    
    # Start broker with mock socket
    broker.start(mock_socket)
    print("✓ Broker started with mock socket")
    
    time.sleep(0.5)  # Let broker start
    
    # Send test commands
    mock_socket.send_command("deauth", "get_status")
    print("✓ Sent get_status command to deauth")
    
    time.sleep(0.5)  # Wait for processing
    
    # Check if response was sent
    assert len(mock_socket.sent_messages) > 0
    print(f"✓ Received {len(mock_socket.sent_messages)} response(s)")
    
    # Parse response
    response = Message.from_json(mock_socket.sent_messages[0])
    print(f"✓ Response parsed: {response.module}.{response.action}")
    
    assert response.module == "deauth"
    print("✓ Response from correct module")
    
    print("\nTest 3 PASSED ✓")
    return mock_socket

def test_deauth_commands():
    """Test 4: Deauth module commands"""
    print("\n" + "="*60)
    print("TEST 4: Deauth Commands")
    print("="*60)
    
    mock_socket = MockSocket()
    broker = get_broker()
    broker.socket = mock_socket
    
    # Test scan_ap command
    print("\nTesting scan_ap command...")
    mock_socket.send_command("deauth", "scan_ap")
    time.sleep(1.0)  # Wait for response
    
    # Should have received response
    assert len(mock_socket.sent_messages) > 0
    response = Message.from_json(mock_socket.sent_messages[-1])
    print(f"✓ Received response: {response.action}")
    
    # Wait for scan to complete (simulated)
    print("Waiting for scan to complete...")
    time.sleep(2.5)
    
    # Should have received scan_complete event
    scan_complete = False
    for msg_json in mock_socket.sent_messages:
        msg = Message.from_json(msg_json)
        if msg.action == "scan_complete":
            scan_complete = True
            print(f"✓ Received scan_complete event")
            break
    
    assert scan_complete
    print("✓ Scan completed successfully")
    
    print("\nTest 4 PASSED ✓")

def test_mitm_commands():
    """Test 5: MITM module commands"""
    print("\n" + "="*60)
    print("TEST 5: MITM Commands")
    print("="*60)
    
    mock_socket = MockSocket()
    broker = get_broker()
    broker.socket = mock_socket
    
    # Test start_poison command
    print("\nTesting start_poison command...")
    data = {
        "target_ip": "192.168.1.100",
        "gateway_ip": "192.168.1.1"
    }
    mock_socket.send_command("mitm", "start_poison", data)
    time.sleep(1.0)
    
    # Should have received response
    assert len(mock_socket.sent_messages) > 0
    response = Message.from_json(mock_socket.sent_messages[-1])
    print(f"✓ Received response: {response.action}")
    
    # Wait for attack progress
    print("Waiting for attack progress...")
    time.sleep(1.5)
    
    # Should have received attack_progress events
    progress_events = 0
    for msg_json in mock_socket.sent_messages:
        msg = Message.from_json(msg_json)
        if msg.action == "attack_progress":
            progress_events += 1
    
    print(f"✓ Received {progress_events} progress event(s)")
    assert progress_events > 0
    
    # Test stop_poison
    print("\nTesting stop_poison command...")
    mock_socket.send_command("mitm", "stop_poison")
    time.sleep(0.5)
    
    print("✓ Stop command processed")
    
    print("\nTest 5 PASSED ✓")

def test_multiple_windows():
    """Test 6: Multiple windows receiving events"""
    print("\n" + "="*60)
    print("TEST 6: Multiple Windows Simulation")
    print("="*60)
    
    mock_socket = MockSocket()
    broker = get_broker()
    broker.socket = mock_socket
    
    # Simulate two windows sending commands simultaneously
    print("\nSimulating Deauth window command...")
    mock_socket.send_command("deauth", "scan_ap")
    
    print("Simulating MITM window command...")
    mock_socket.send_command("mitm", "get_status")
    
    time.sleep(1.0)
    
    # Both should have responses
    deauth_response = False
    mitm_response = False
    
    for msg_json in mock_socket.sent_messages:
        msg = Message.from_json(msg_json)
        if msg.module == "deauth" and msg.type == "RESP":
            deauth_response = True
        if msg.module == "mitm" and msg.type == "RESP":
            mitm_response = True
    
    assert deauth_response and mitm_response
    print("✓ Both modules responded independently")
    
    print("\nTest 6 PASSED ✓")

def run_all_tests():
    """Run all tests"""
    print("\n" + "#"*60)
    print("#" + " "*58 + "#")
    print("#" + "  Insidious Message Broker Test Suite".center(58) + "#")
    print("#" + " "*58 + "#")
    print("#"*60)
    
    try:
        # Test 1
        broker = test_broker_initialization()
        
        # Test 2
        deauth_mod, mitm_mod = test_module_initialization()
        
        # Test 3
        mock_socket = test_message_routing()
        
        # Test 4
        test_deauth_commands()
        
        # Test 5
        test_mitm_commands()
        
        # Test 6
        test_multiple_windows()
        
        # Cleanup
        print("\n" + "="*60)
        print("CLEANUP")
        print("="*60)
        deauth_mod.stop()
        mitm_mod.stop()
        broker.stop()
        print("✓ All modules stopped")
        
        # Summary
        print("\n" + "#"*60)
        print("#" + " "*58 + "#")
        print("#" + "  ALL TESTS PASSED! ✓".center(58) + "#")
        print("#" + " "*58 + "#")
        print("#"*60)
        
        print("\nYour architecture is working correctly!")
        print("\nNext steps:")
        print("  1. Start python_bridge.py")
        print("  2. Start your C# GUI")
        print("  3. Test with actual GUI interactions")
        
        return True
        
    except Exception as e:
        print("\n" + "!"*60)
        print("!" + " "*58 + "!")
        print("!" + f"  TEST FAILED: {str(e)}".ljust(58) + "!")
        print("!" + " "*58 + "!")
        print("!"*60)
        import traceback
        traceback.print_exc()
        return False

if __name__ == "__main__":
    success = run_all_tests()
    sys.exit(0 if success else 1)
